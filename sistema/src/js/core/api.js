import axios from "axios";
import config from "../../assets/configs/config.json";
import Result from "./result.js";
import { Storage } from "./storage";
import qs from "qs";
import ObjectExtension from "../utils/objects/object-extensions.js";
import { AuthService } from "./auth";
import JsonWrapper from "../utils/jsons/json-wrapper.js";

const DEFAULT_TIMEOUT = 1000 * 60 * 5; // 5 minutes

axios.defaults.timeout = DEFAULT_TIMEOUT;
export class ApiService {
  constructor() {
    this.api = axios.create({
      baseURL: config.apiUrl,
      timeout: DEFAULT_TIMEOUT,
    });
  }

  async get(endpoint, params = {}, headers = {}) {
    const loadingOverlay = document.getElementById("loading-overlay");
    try {
      headers ??= { "Content-Type": "application/json" };

      if (!headers["Content-Type"]) {
        headers["Content-Type"] = "application/json";
      }

      headers["Authorization"] = `Bearer ${Storage.getItem("token")}`;

      this.showLoadingOverlay(loadingOverlay);

      params = ObjectExtension.removeEmptyProperties(params);

      const response = await this.api.get(endpoint, {
        params,
        headers: headers,
        withCredentials: true,
        timeout: DEFAULT_TIMEOUT,
        paramsSerializer: (params) => {
          return qs.stringify(params, {
            arrayFormat: "indices", // Use 'indices' to prevent brackets
            encode: false, // To prevent encoding values
          });
        },
      });
      this.hideLoadingOverlay(loadingOverlay);

      return Result.success(response.data);
    } catch (error) {
      this.hideLoadingOverlay(loadingOverlay);
      const result = await this.handleError(error);
      return result !== null ?
        result :
        Result.failure("Não foi possível processar a requisição");
    }
  }

  async delete(endpoint, params = {}, headers = {}) {
    const loadingOverlay = document.getElementById("loading-overlay");
    try {
      headers ??= { "Content-Type": "application/json" };

      if (!headers["Content-Type"]) {
        headers["Content-Type"] = "application/json";
      }

      headers["Authorization"] = `Bearer ${Storage.getItem("token")}`;

      this.showLoadingOverlay(loadingOverlay);

      params = ObjectExtension.removeEmptyProperties(params);

      const response = await this.api.delete(endpoint, {
        params,
        headers: headers,
        withCredentials: true,
        timeout: DEFAULT_TIMEOUT,
        paramsSerializer: (params) => {
          return qs.stringify(params, {
            arrayFormat: "indices", // Use 'indices' to prevent brackets
            encode: false, // To prevent encoding values
          });
        },
      });
      this.hideLoadingOverlay(loadingOverlay);

      return Result.success(response.data);
    } catch (error) {
      this.hideLoadingOverlay(loadingOverlay);
      const result = await this.handleError(error);
      return result !== null ?
        result :
        Result.failure("Não foi possível processar a requisição");
    }
  }

  async download(endpoint, data = {}, headers = {}) {
    const loadingOverlay = document.getElementById("loading-overlay");
    try {
      headers ??= { "Content-Type": "application/json" };

      if (!headers["Content-Type"]) {
        headers["Content-Type"] = "application/json";
      }

      headers["Authorization"] = `Bearer ${Storage.getItem("token")}`;

      this.showLoadingOverlay(loadingOverlay);

      const response = await this.api.post(endpoint, data, {
        headers: headers,
        withCredentials: true,
        responseType: 'blob',
        timeout: DEFAULT_TIMEOUT,
        paramsSerializer: (params) => {
          return qs.stringify(params, {
            arrayFormat: "indices", // Use 'indices' to prevent brackets
            encode: false, // To prevent encoding values
          });
        },
      });

      this.hideLoadingOverlay(loadingOverlay);

      let filename = ""
      let disposition = response.headers['content-disposition']

      if (disposition && disposition.indexOf('attachment') !== -1) {
          const filenameRegex = /filename[^=\n]*=((['"]).*?\2|[^\n]*)/
          const matches = filenameRegex.exec(disposition)

          if (matches != null && matches[1]) {
            filename = ((matches[1].replace(/['"]/g, '')).split("; "))[0]
          }
      }

      const result = {
        contentType: response.headers["content-type"],
        data: {
          filename,
          blob: response.data
        },
      };

      return Result.success(result);
    } catch (error) {
      this.hideLoadingOverlay(loadingOverlay);
      const result = await this.handleError(error);
      return result !== null ?
        result :
        Result.failure("Não foi possível processar a requisição");
    }
  }

  async downloadBlob(endpoint, params = {}, headers = {}) {
    const loadingOverlay = document.getElementById("loading-overlay");
    try {
      headers ??= { "Content-Type": "application/json" };

      if (!headers["Content-Type"]) {
        headers["Content-Type"] = "application/json";
      }

      headers["Authorization"] = `Bearer ${Storage.getItem("token")}`;

      this.showLoadingOverlay(loadingOverlay);

      params = ObjectExtension.removeEmptyProperties(params);

      const response = await this.api.get(endpoint, {
        params,
        headers: headers,
        withCredentials: true,
        responseType: "blob",
        timeout: DEFAULT_TIMEOUT,
        paramsSerializer: (params) => {
          return qs.stringify(params, {
            arrayFormat: "indices", // Use 'indices' to prevent brackets
            encode: false, // To prevent encoding values
          });
        },
      });

      this.hideLoadingOverlay(loadingOverlay);

      const result = {
        contentType: response.headers["content-type"],
        data: response.data,
      };

      return Result.success(result);
    } catch (error) {
      this.hideLoadingOverlay(loadingOverlay);
      const result = await this.handleError(error);
      return result !== null ?
        result :
        Result.failure("Não foi possível processar a requisição");
    }
  }

  async post(endpoint, data = {}, headers = {}) {
    const loadingOverlay = document.getElementById("loading-overlay");

    try {
      headers ??= { "Content-Type": "application/json" };

      if (!headers["Content-Type"]) {
        headers["Content-Type"] = "application/json";
      }

      if (
        (headers["Content-Type"] === "multipart/form-data" ||
          headers["Content-Type"] === "application/x-www-form-urlencoded") &&
        data
      ) {
        data = JsonWrapper.jsonToFormData(data);
      }

      if (!headers["Accept"]) {
        headers["Accept"] = "application/json";
      }

      headers["Authorization"] = `Bearer ${Storage.getItem("token")}`;

      this.showLoadingOverlay(loadingOverlay);
      const response = await this.api.post(endpoint, data, {
        headers: headers,
        withCredentials: true,
        timeout: DEFAULT_TIMEOUT,
      });
      this.hideLoadingOverlay(loadingOverlay);

      return Result.success(response.data);
    } catch (error) {
      this.hideLoadingOverlay(loadingOverlay);

      const result = await this.handleError(error);
      return result !== null ?
        result :
        Result.failure("Não foi possível processar a requisição");
    }
  }

  async patch(endpoint, data = {}, headers = {}) {
    const loadingOverlay = document.getElementById("loading-overlay");

    try {
      headers ??= { "Content-Type": "application/json" };

      if (!headers["Content-Type"]) {
        headers["Content-Type"] = "application/json";
      }

      if (
        (headers["Content-Type"] === "multipart/form-data" ||
          headers["Content-Type"] === "application/x-www-form-urlencoded") &&
        data
      ) {
        const formData = new URLSearchParams();

        for (const key in data) {
          // eslint-disable-next-line no-prototype-builtins
          if (data.hasOwnProperty(key)) {
            formData.append(key, data[key]);
          }
        }

        data = formData;
      }

      if (!headers["Accept"]) {
        headers["Accept"] = "application/json";
      }

      headers["Authorization"] = `Bearer ${Storage.getItem("token")}`;

      this.showLoadingOverlay(loadingOverlay);
      const response = await this.api.patch(endpoint, data, {
        headers: headers,
        withCredentials: true,
        timeout: DEFAULT_TIMEOUT,
      });
      this.hideLoadingOverlay(loadingOverlay);

      return Result.success(response.data);
    } catch (error) {
      this.hideLoadingOverlay(loadingOverlay);

      const result = await this.handleError(error);
      return result !== null ?
        result :
        Result.failure("Não foi possível processar a requisição");
    }
  }

  async put(endpoint, data = {}, headers = {}) {
    const loadingOverlay = document.getElementById("loading-overlay");

    try {
      headers ??= { "Content-Type": "application/json" };

      if (!headers["Content-Type"]) {
        headers["Content-Type"] = "application/json";
      }

      if (
        (headers["Content-Type"] === "multipart/form-data" ||
          headers["Content-Type"] === "application/x-www-form-urlencoded") &&
        data
      ) {
        data = JsonWrapper.jsonToFormData(data);
      }

      headers["Authorization"] = `Bearer ${Storage.getItem("token")}`;

      this.showLoadingOverlay(loadingOverlay);
      const response = await this.api.put(endpoint, data, {
        headers: headers,
        withCredentials: true,
        timeout: DEFAULT_TIMEOUT,
      });
      this.hideLoadingOverlay(loadingOverlay);

      return Result.success(response.data);
    } catch (error) {
      this.hideLoadingOverlay(loadingOverlay);
      const result = await this.handleError(error);
      return result !== null ?
        result :
        Result.failure("Não foi possível processar a requisição");
    }
  }

  showLoadingOverlay(overlayElement) {
    overlayElement.style.display = "flex";
  }

  hideLoadingOverlay(overlayElement) {
    overlayElement.style.display = "none";
  }

  async handleError(error) {
    if (!error || !error.response) {
      console.log(error);
      return;
    }

    if (error.response.status === 401) {
      await AuthService.initialize();
    }

    if (error.response.status === 403) {
      return Result.failure("Você não tem permissão para acessar este recurso");
    }

    if (error.response.status === 409) {
      return Result.failure("Já existe um registro com os dados informados");
    }

    let errorMessageFromBlob = null;

    if (error.response &&
      error.response.data &&
      error.response.data instanceof Blob &&
      error.response.data.type &&
      error.response.data.type.toLowerCase().indexOf('json') != -1) {
      errorMessageFromBlob = JSON.parse(await error.response.data.text());
      errorMessageFromBlob = errorMessageFromBlob?.errors[0]?.message;
      }

    const statusCodesErrors = [400, 500];

    if (statusCodesErrors.indexOf(error.response.status) >= 0 &&
      error.response.data &&
      (
        errorMessageFromBlob ||
        (error.response.data.errors && error.response.data.errors.length > 0)
      )
    ) {
      return Result.failure(errorMessageFromBlob ?? error.response.data.errors[0]?.message);
    }

    return Result.failure("Não foi possível processar a requisição");
  }
}
