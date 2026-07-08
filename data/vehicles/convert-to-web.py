import json
from pathlib import Path
import sys
import uuid


def salt():
    return uuid.uuid4().hex[:8]


def process_node(node, level=0):
    # Захист: якщо вузол не є словником (наприклад, це рядок чи число), повертаємо як є
    if not isinstance(node, dict):
        return node

    is_leaf = "children" not in node

    # Якщо вузол має дітей, додаємо showCheckbox: false
    if not is_leaf:
        node["showCheckbox"] = False
        # Перевіряємо, чи children є списком перед ітерацією
        if isinstance(node["children"], list):
            node["children"] = [
                process_node(child, level + 1) for child in node["children"]
            ]

    # Якщо немає "value", безпечно створюємо з label
    if "value" not in node:
        label_val = node.get("label", "unknown")
        node["value"] = f"{level}_{label_val}"

    # Додаємо унікальний сіль для кінцевих вузлів (leaves)
    if is_leaf:
        node["value"] = f"{node['value']}#{salt()}"

    return node


def process_json_file(input_path, output_path):
    try:
        with open(input_path, "r", encoding="utf-8") as f:
            data = json.load(f)

        if isinstance(data, list):
            result = [process_node(item, level=0) for item in data]
        else:
            result = process_node(data, level=0)

        out_path = Path(output_path)
        out_path.parent.mkdir(parents=True, exist_ok=True)
        with open(out_path, "w", encoding="utf-8") as f:
            json.dump(result, f, ensure_ascii=False, indent=2)

        print(f" Successfully processed: {input_path.name} -> {out_path}")
    except Exception as e:
        print(f"❌ Error processing {input_path.name}: {e}", file=sys.stderr)


if __name__ == "__main__":
    if len(sys.argv) < 3:
        print("Usage: python convert-to-web.py <input_dir> <output_dir>")
        sys.exit(1)

    # Виправлено: отримуємо конкретні індекси з sys.argv
    input_path = Path(sys.argv[1])
    output_path = Path(sys.argv[2])

    output_path.mkdir(parents=True, exist_ok=True)

    extension = "*.json"
    files = list(input_path.glob(extension))
    print(f"Found files: {files}")

    for file in files:
        process_json_file(file, output_path / file.name)
