import axios from 'axios';
import config from '../../assets/configs/config.json'
import Result from './result.js';

export class ApiLegadoService {
    constructor() {
        this.api = axios.create({
            baseURL: config.url,
        });
    }

    async get(endpoint, params = {}, headers = {}) {
        const loadingOverlay = document.getElementById('loading-overlay');
        try {
            headers ??= { 'Content-Type': 'application/json' };

            if (!headers['Content-Type']) {
                headers['Content-Type'] = 'application/json';
            }

            this.showLoadingOverlay(loadingOverlay);
            const response = await this.api.get(endpoint, { params, headers: headers, withCredentials: true });
            this.hideLoadingOverlay(loadingOverlay);

            return Result.success(response.data);
        } catch (error) {
            this.hideLoadingOverlay(loadingOverlay);
            this.handleError(error);
            return Result.failure(error);
        }
    }

    async post(endpoint, data = {}, headers = {}) {
        const loadingOverlay = document.getElementById('loading-overlay');

        try {

            headers ??= { 'Content-Type': 'application/json' };

            if (!headers['Content-Type']) {
                headers['Content-Type'] = 'application/json';
            }

            if ((headers['Content-Type'] === 'multipart/form-data' ||
                headers['Content-Type'] === 'application/x-www-form-urlencoded') && data) {
                if (!data.fullfield) {
                    const formData = new URLSearchParams();

                    for (const key in data) {
                        // eslint-disable-next-line no-prototype-builtins
                        if (data.hasOwnProperty(key)) {
                            formData.append(key, data[key]);
                        }
                    }

                    data = formData;
                }
            }

            this.showLoadingOverlay(loadingOverlay);
            const response = await this.api.post(endpoint, data, { headers: headers, withCredentials: true });
            this.hideLoadingOverlay(loadingOverlay);

            return Result.success(response.data);
        } catch (error) {
            this.hideLoadingOverlay(loadingOverlay);
            this.handleError(error);
            return Result.failure(error);
        }
    }

    showLoadingOverlay(overlayElement) {
        overlayElement.style.display = 'flex';
    }

    hideLoadingOverlay(overlayElement) {
        overlayElement.style.display = 'none';
    }

    handleError(error) {
        // Implement your custom error handling logic here
        // You can log errors, show notifications, etc.
        console.log(error);
    }
}