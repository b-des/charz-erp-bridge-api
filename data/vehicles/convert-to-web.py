import json
from pathlib import Path
import sys
import uuid


def salt():
    return uuid.uuid4().hex[:8]


def process_node(node, level=0):
    is_leaf = "children" not in node

    # If node has children, add showCheckbox: false
    if not is_leaf:
        node["showCheckbox"] = False
        node["children"] = [process_node(child, level + 1) for child in node["children"]]

    # If node has no "value" property, copy label to value with level index prefix
    if "value" not in node:
        node["value"] = f"{level}_{node.get('label', '')}"

    # Append salt to leaf node values
    if is_leaf:
        node["value"] = f"{node['value']}#{salt()}"

    return node


def process_json_file(input_path, output_path=None):
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

    print(f"Done. Saved to {out_path}")


if __name__ == "__main__":
    if len(sys.argv) < 2:
        print("Usage: python convert-to-web.py <input> [output]")
        sys.exit(1)

    input_path = Path(sys.argv[1])
    output_path = Path(sys.argv[2]) if len(sys.argv) > 2 else None
    extension = "*.json"
    files = list(input_path.glob(extension))
    print(files)
    for file in files:
        process_json_file(file, output_path / file.name)

