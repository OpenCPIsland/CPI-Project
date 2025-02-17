import json
from googletrans import Translator
import asyncio

translator = Translator()

languages = {
    "es_LA": "es",
    "fr_FR": "fr",
    "pt_BR": "pt"
}

async def translate_item(value, code):
    translation = await translator.translate(value, dest=code)
    return translation.text

async def translate_all():
    with open('en_US.json.json', 'r', encoding='utf-8') as f:
        en_data = json.load(f)

    total_keys = len(en_data)
    for lang, code in languages.items():
        translated_data = {}
        tasks = []

        # Create tasks to translate all items in parallel
        for key, value in en_data.items():
            task = translate_item(value, code)
            tasks.append((key, task))

        # Gather all translations in parallel
        translations = await asyncio.gather(*[task[1] for task in tasks])

        # Store translated items and show progress
        for idx, (key, _) in enumerate(tasks):
            translated_data[key] = translations[idx]

            # Calculate and print progress
            progress = (idx + 1) / total_keys * 100
            print(f"Translating {lang}... {progress:.2f}% complete", end="\r")

        with open(f'{lang}.json.json', 'w', encoding='utf-8') as f:
            json.dump(translated_data, f, ensure_ascii=False, indent=4)

        print(f"\n{lang} translation completed!")

    print("All translations completed!")

# Run the async function
asyncio.run(translate_all())
