export class Storage {
    static getItem(key) {
        return sessionStorage.getItem(key);
    }

    static setItem(key, value) {
        value = typeof value === 'object' ? JSON.stringify(value) : value;
        sessionStorage.setItem(key, value);
    }

    static removeItem(key) {
        sessionStorage.removeItem(key);
    }

    static clear() {
        sessionStorage.clear();
    }

    static getObject(key){
        const value = sessionStorage.getItem(key);

        return value && JSON.parse(value);
    }
}