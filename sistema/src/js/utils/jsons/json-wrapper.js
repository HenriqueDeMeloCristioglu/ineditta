export default class JsonWrapper {
  static jsonToFormData(json) {
    const formData = new FormData();

    function appendFormData(key, value) {
      if (value instanceof File) {
        formData.append(key, value);
      } else if (
        typeof value === "number" &&
        Number.isFinite(value) &&
        Number.isInteger(value)
      ) {
        formData.append(key, value.toString());
      } else if (typeof value === "number") {
        formData.append(key, value.toString().replace(".", ","));
      } else {
        formData.append(key, value.toString());
      }
    }

    Object.keys(json).forEach((key) => {
      const value = json[key];
      if (!value) return;

      if (Array.isArray(value)) {
        processArray(key, value);
      } else {
        appendFormData(key, value);
      }
    });

    function processArray(key, array) {
        array.map((item, index) => {
        
        if (item instanceof Object) {
            Object.keys(item).map((itemKey) => {
              const itemValue = item[itemKey];
              if (Array.isArray(itemValue)) {
                processArray(`${key}[${index}][${itemKey}]`, itemValue);
              } else {
                appendFormData(`${key}[${index}][${itemKey}]`, itemValue);
              }
            });
        }
        else {
            appendFormData(`${key}[${index}]`, item);
        }

      });
    }

    return formData;
  }
}
