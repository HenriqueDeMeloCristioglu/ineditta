export default class Uri {
    static getQueryParams(url) {
        if (!url) {
            return null;
        }

        const urlObj = new URL(url);
        const params = new URLSearchParams(urlObj.search);

        // Extracting all the query parameters into an object
        const queryParams = {};
        for (let [key, value] of params.entries()) {
            queryParams[key] = value;
        }

        return queryParams;
    }
}