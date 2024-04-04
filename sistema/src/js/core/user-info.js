import { Storage } from './storage.js';
export class UserInfoService {
    static getEmail() {
        const userInfo = Storage.getItem('userInfo');

        if (userInfo) {
            return JSON.parse(userInfo);
        }

        return null;
    }

    static getName() {
        const userInfo = Storage.getItem('userInfo');

        if (userInfo) {
            return JSON.parse(userInfo).username;
        }

        return null;
    }

    static getId() {
        const userInfo = Storage.getItem('userInfo');

        if (userInfo) {
            return JSON.parse(userInfo).sub;
        }

        return null;
    }

    static getProfile() {
        const userInfo = Storage.getItem('userInfo');

        if (userInfo) {
            return JSON.parse(userInfo);
        }

        return null;
    }

    static getEconomicGroup() {
        return Storage.getItem('grupoecon');
    }

    static getUserId() {
        return Storage.getItem('iduser');
    }

    static getTipo() {
        return Storage.getItem('tipo');
    }
    
    static getFirstName() {
        const userInfo = Storage.getItem('userInfo');

        if (userInfo) {
            return JSON.parse(userInfo).firstName;
        }

        return null;
    }

    static getLastName() {
        const userInfo = Storage.getItem('userInfo');

        if (userInfo) {
            return JSON.parse(userInfo).lastName;
        }

        return null;
    }

    static getUserName() {
        return Storage.getItem('nome_usuario');
    }

    static getDataUser() {
        return Storage.getItem('data_user');
    }
}