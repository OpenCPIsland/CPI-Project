#!/bin/bash

gnome-terminal -- bash -c "
VENV_DIR='venv'

if [ ! -d \"\$VENV_DIR\" ]; then
    python3 -m venv \"\$VENV_DIR\"
fi

source \"\$VENV_DIR/bin/activate\"

pip install --upgrade pip

pip install torch torchvision torchaudio --index-url https://download.pytorch.org/whl/cpu
pip install transformers sentencepiece tqdm protobuf safetensors sacremoses huggingface_hub hf-xet

if [ -n "$TRANSLATION_DIR" ]; then
    python3 translate.py --output-dir "$TRANSLATION_DIR"
else
    python3 translate.py
fi

echo ''
echo 'Translation finished. Press Enter to close...'
read
"
