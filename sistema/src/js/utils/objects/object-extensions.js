export default class ObjectExtension {
    static removeEmptyProperties(obj) {
        if(!obj || typeof obj !== 'object'){
            return obj;
        }

        for (let key in obj) {
            if (obj[key] === null || obj[key] === '') {
                delete obj[key];
            } else if (typeof obj[key] === 'object') {
                ObjectExtension.removeEmptyProperties(obj[key]);
            }
        }

        return obj;
    }
}