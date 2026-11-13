import os

os.environ.setdefault("HF_HUB_DISABLE_SYMLINKS_WARNING", "1")

import sys
import argparse
import json
from pathlib import Path
import torch
from transformers import MarianMTModel, MarianTokenizer
from tqdm import tqdm

if os.name == "nt":
    try:
        sys.stdout.reconfigure(encoding="utf-8")
    except Exception:
        pass

SCRIPT_DIR = Path(__file__).resolve().parent
INVOCATION_DIR = Path.cwd()
os.chdir(SCRIPT_DIR)

SOURCE_FILE_CANDIDATES = (
    "en_US.json.json",
)
EXPECTED_SOURCE_PARENT = ("Assets", "Generated", "Resources", "Translations", "Global")
LOCALE_FILE_SUFFIXES = (".json.json",)

DEFAULT_BATCH_SIZE_CPU = 32
DEFAULT_BATCH_SIZE_GPU = 32

USE_FP16_ON_GPU = True

USE_GREEDY_DECODE = True

env_cpu_threads = os.environ.get("TRANSLATE_CPU_THREADS")
if env_cpu_threads:
    try:
        torch.set_num_threads(max(1, int(env_cpu_threads)))
    except Exception:
        pass

languages = {
    "es_LA": "es",
    "fr_FR": "fr",
    "pt_BR": "pt"
}

hf_model_map = {
    "es": "Helsinki-NLP/opus-mt-en-es",
    "fr": "Helsinki-NLP/opus-mt-en-fr",
    "pt": "Helsinki-NLP/opus-mt-tc-big-en-pt"
}

MODEL_DIR = SCRIPT_DIR / "models"
MODEL_DIR.mkdir(exist_ok=True)

if torch.cuda.is_available():
    device = "cuda"
    gpu_available = True
elif getattr(torch.version, "hip", None) is not None:
    device = "cuda"
    gpu_available = True
else:
    device = "cpu"
    gpu_available = False

env_batch = os.environ.get("TRANSLATE_BATCH_SIZE")
if env_batch:
    try:
        BATCH_SIZE = int(env_batch)
    except Exception:
        BATCH_SIZE = DEFAULT_BATCH_SIZE_GPU if gpu_available else DEFAULT_BATCH_SIZE_CPU
else:
    BATCH_SIZE = DEFAULT_BATCH_SIZE_GPU if gpu_available else DEFAULT_BATCH_SIZE_CPU

print("Using device:", device)
print("GPU available:", gpu_available)
print("Batch size:", BATCH_SIZE)
print()

def download_or_update_models(target_languages):
    prepared_codes = set()
    for lang_code, model_code in target_languages.items():
        if model_code in prepared_codes:
            continue

        model_name = hf_model_map[model_code]
        path = MODEL_DIR / model_code
        print(f"Downloading or updating model for {lang_code}...")
        MarianMTModel.from_pretrained(model_name, cache_dir=path, force_download=False)
        MarianTokenizer.from_pretrained(model_name, cache_dir=path, force_download=False)
        prepared_codes.add(model_code)

    print("All models ready.\n")

def translate_batch(batch, model, tokenizer, device):
    results = {}
    texts = [v for _, v in batch]
    keys = [k for k, _ in batch]

    encoded = tokenizer(texts, return_tensors="pt", truncation=True, padding=True)
    encoded = {k: v.to(device) for k, v in encoded.items()}

    gen_kwargs = {}
    if USE_GREEDY_DECODE:
        gen_kwargs["num_beams"] = 1

    with torch.inference_mode():
        output = model.generate(**encoded, **gen_kwargs)

    decoded = tokenizer.batch_decode(output, skip_special_tokens=True)
    for k, t in zip(keys, decoded):
        results[k] = t
    return results

def translate_language(data, lang_code, model_code, device):
    model_name = hf_model_map[model_code]
    path = MODEL_DIR / model_code

    print(f"Loading model for {lang_code}...")
    tokenizer = MarianTokenizer.from_pretrained(model_name, cache_dir=path)
    model = MarianMTModel.from_pretrained(model_name, cache_dir=path)

    model.to(device)
    if device != "cpu" and USE_FP16_ON_GPU:
        try:
            model.half()
            print("Model converted to FP16 (half precision).")
        except Exception:
            print("FP16 conversion failed or not supported; continuing with full precision.")

    model.eval()

    items = list(data.items())
    total_entries = len(items)
    batches = [items[i:i+BATCH_SIZE] for i in range(0, total_entries, BATCH_SIZE)]
    print(f"{lang_code}: {total_entries} entries, {len(batches)} batches\n")

    output_data = {}

    for batch in tqdm(batches, desc=f"Translating {lang_code}", ncols=100):
        result = translate_batch(batch, model, tokenizer, device)
        output_data.update(result)
        if device != "cpu":
            try:
                torch.cuda.empty_cache()
            except Exception:
                pass

    return output_data

def parse_args():
    parser = argparse.ArgumentParser(
        description="Sync sibling locale files from English source strings."
    )
    parser.add_argument(
        "--source",
        help="Path to the English source JSON file. Defaults to auto-discovery.",
    )
    parser.add_argument(
        "--output-dir",
        help="Directory where sibling locale files should be written. Defaults to the source file folder.",
    )
    parser.add_argument(
        "--languages",
        help="Comma-separated locale file names to translate, like pt_BR or es_LA,fr_FR.",
    )
    parser.add_argument(
        "--quality",
        default="auto",
        help="Accepted for launcher compatibility. Marian models ignore this value.",
    )
    parser.add_argument(
        "--dry-run",
        action="store_true",
        help="Resolve paths and exit without downloading models or translating.",
    )
    return parser.parse_args()


def looks_like_translation_source(path):
    return (
        path.is_file()
        and path.name.lower() in {name.lower() for name in SOURCE_FILE_CANDIDATES}
        and tuple(path.parent.parts[-5:]) == EXPECTED_SOURCE_PARENT
    )


def find_named_file(directory, names):
    if directory is None or not directory.exists() or not directory.is_dir():
        return None

    lower_name_map = {name.lower() for name in names}

    for name in names:
        candidate = directory / name
        if candidate.is_file():
            return candidate.resolve()

    for candidate in directory.iterdir():
        if candidate.is_file() and candidate.name.lower() in lower_name_map:
            return candidate.resolve()

    return None


def resolve_input_path(value):
    path = Path(value).expanduser()
    if not path.is_absolute():
        path = INVOCATION_DIR / path
    return path.resolve()


def resolve_locale_path(output_dir, locale):
    path = find_named_file(output_dir, [f"{locale}{suffix}" for suffix in LOCALE_FILE_SUFFIXES])
    if path is not None:
        return path
    return (output_dir / f"{locale}.json.json").resolve()


def load_json_object(path):
    with open(path, "r", encoding="utf-8") as f:
        data = json.load(f)

    if not isinstance(data, dict):
        raise ValueError(f"Expected a JSON object in {path}, got {type(data).__name__}.")

    return data


def recover_flat_json_object(path):
    recovered = {}
    saw_non_whitespace = False

    with open(path, "r", encoding="utf-8", errors="replace") as f:
        for line_number, line in enumerate(f, start=1):
            stripped = line.strip()
            if not stripped:
                continue

            saw_non_whitespace = True
            if stripped in ("{", "}"):
                continue

            entry_source = stripped[:-1] if stripped.endswith(",") else stripped

            try:
                entry = json.loads("{" + entry_source + "}")
            except json.JSONDecodeError:
                print(f"Stopped recovering {path.name} at line {line_number}.")
                break

            if not isinstance(entry, dict) or len(entry) != 1:
                print(f"Stopped recovering {path.name} at line {line_number}.")
                break

            key, value = next(iter(entry.items()))
            recovered[key] = value

    if recovered:
        return recovered
    if not saw_non_whitespace:
        return {}
    return None


def load_locale_data(path):
    if not path.exists():
        return {}, False, False

    try:
        return load_json_object(path), False, True
    except json.JSONDecodeError as exc:
        recovered = recover_flat_json_object(path)
        if recovered is None:
            raise ValueError(f"Failed to parse {path}: {exc}") from exc

        print(
            f"Warning: {path.name} is not valid JSON ({exc}). "
            f"Recovered {len(recovered)} existing entries."
        )
        return recovered, True, True


def build_language_plan(source_data, existing_data, lang_code, model_code, target_path, file_exists, recovered):
    missing_entries = {}
    missing_passthrough = {}

    for key, value in source_data.items():
        if key in existing_data:
            continue
        if isinstance(value, str):
            missing_entries[key] = value
        else:
            missing_passthrough[key] = value

    extra_keys = [key for key in existing_data.keys() if key not in source_data]

    return {
        "lang_code": lang_code,
        "model_code": model_code,
        "target_path": target_path,
        "file_exists": file_exists,
        "recovered": recovered,
        "existing_data": existing_data,
        "missing_entries": missing_entries,
        "missing_passthrough": missing_passthrough,
        "extra_keys": extra_keys,
        "needs_write": (not file_exists) or recovered or bool(missing_entries) or bool(missing_passthrough),
    }


def build_merged_output(source_data, existing_data, translated_entries):
    merged = {}

    for key, value in source_data.items():
        if key in existing_data:
            merged[key] = existing_data[key]
        else:
            merged[key] = translated_entries.get(key, value)

    for key, value in existing_data.items():
        if key not in source_data:
            merged[key] = value

    return merged


def write_locale_data(path, data):
    with open(path, "w", encoding="utf-8") as f:
        json.dump(data, f, ensure_ascii=False, indent=2)
        f.write("\n")


def find_default_source():
    search_roots = []
    current = SCRIPT_DIR
    for _ in range(3):
        if current not in search_roots:
            search_roots.append(current)
        if current.parent == current:
            break
        current = current.parent

    direct_candidates = []
    for root in search_roots:
        for source_name in SOURCE_FILE_CANDIDATES:
            direct_candidate = root / source_name
            if direct_candidate not in direct_candidates:
                direct_candidates.append(direct_candidate)

            nested_candidate = root / Path(*EXPECTED_SOURCE_PARENT) / source_name
            if nested_candidate not in direct_candidates:
                direct_candidates.append(nested_candidate)

    for candidate in direct_candidates:
        if candidate.is_file() and looks_like_translation_source(candidate):
            return candidate

    for root in search_roots:
        for source_name in SOURCE_FILE_CANDIDATES:
            for candidate in root.rglob(source_name):
                if looks_like_translation_source(candidate):
                    return candidate

    return None


def resolve_paths(args):
    if args.output_dir:
        output_dir = resolve_input_path(args.output_dir)
    else:
        output_dir = None

    if args.source:
        source_path = resolve_input_path(args.source)
    else:
        source_path = find_named_file(output_dir, SOURCE_FILE_CANDIDATES) if output_dir else None
        if source_path is None:
            source_path = find_default_source()

    if source_path is None:
        raise FileNotFoundError(
            "Could not find an English source JSON file. Pass --source, or point --output-dir at the "
            "Translations/Global folder that contains en_US.json.json."
        )

    if not source_path.exists():
        raise FileNotFoundError(f"Missing source file: {source_path}")

    if output_dir is None:
        output_dir = source_path.parent

    output_dir.mkdir(parents=True, exist_ok=True)
    return source_path, output_dir


def resolve_target_languages(args):
    if not args.languages:
        return languages

    requested = {}
    for item in args.languages.split(","):
        locale = item.strip()
        if not locale:
            continue
        if locale not in languages:
            raise ValueError(f"Unsupported language: {locale}")
        requested[locale] = languages[locale]

    if not requested:
        raise ValueError("No valid languages were provided.")

    return requested


def main():
    args = parse_args()

    try:
        source_path, output_dir = resolve_paths(args)
        target_languages = resolve_target_languages(args)
    except Exception as exc:
        print(exc)
        return 1

    print("Source file:", source_path)
    print("Output dir:", output_dir)
    print("Targets:", ", ".join(target_languages.keys()))
    print()

    if args.dry_run:
        print("Dry run complete.")
        return 0

    try:
        data = load_json_object(source_path)
    except Exception as e:
        print("Failed to parse JSON source file:", e)
        return 1

    language_plans = {}
    for lang, code in target_languages.items():
        target_path = resolve_locale_path(output_dir, lang)

        try:
            existing_data, recovered, file_exists = load_locale_data(target_path)
        except Exception as e:
            print(f"Failed to load target file for {lang}: {e}")
            return 1

        plan = build_language_plan(data, existing_data, lang, code, target_path, file_exists, recovered)
        language_plans[lang] = plan

        print(
            f"{lang}: {len(existing_data)} existing, "
            f"{len(plan['missing_entries']) + len(plan['missing_passthrough'])} missing, "
            f"{len(plan['extra_keys'])} extra"
        )
        if recovered:
            print(f"{lang}: recovered from a truncated or malformed file.")
        if not plan["needs_write"]:
            print(f"{lang}: no missing entries; leaving existing translations untouched.")
        print()

    languages_needing_translation = {
        lang: plan["model_code"]
        for lang, plan in language_plans.items()
        if plan["missing_entries"]
    }

    if languages_needing_translation:
        download_or_update_models(languages_needing_translation)

    for lang, plan in language_plans.items():
        if not plan["needs_write"]:
            continue

        translated_entries = {}
        if plan["missing_entries"]:
            translated_entries = translate_language(
                plan["missing_entries"],
                plan["lang_code"],
                plan["model_code"],
                device,
            )

        merged_output = build_merged_output(
            data,
            plan["existing_data"],
            translated_entries,
        )
        write_locale_data(plan["target_path"], merged_output)

        print(
            f"{lang}: wrote {plan['target_path']} "
            f"with {len(plan['missing_entries']) + len(plan['missing_passthrough'])} new entries."
        )
        print()

    print("All translations completed.")
    return 0

if __name__ == "__main__":
    raise SystemExit(main())
