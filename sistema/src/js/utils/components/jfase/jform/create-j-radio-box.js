import { stringDiv, stringInput, stringLabel } from "../../string-elements";

export function createJRadioBox({ id, name, value, checked = false }) {
  const val = String(value).replace(/[^a-zA-Z0-9]/g, "");

  return stringDiv({
    className: "form-check",
    children:
      stringInput({
        id,
        className: "form-check-input",
        type: "radio",
        checked,
        name,
        value: val,
      }) +
      stringLabel({
        id,
        className: "form-check-label",
        text: value,
      }),
  });
}
