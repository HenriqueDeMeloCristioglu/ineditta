export default class Result {
    constructor(ok, value, error) {
        this.success = ok;
        if (ok) {
            this.value = value;
        } else {
            this.error = error;
        }
    }

    static success(value) {
        return new Result(true, value);
    }

    static failure(error) {
        return new Result(false, undefined, error);
    }

    isSuccess() {
        return this.success;
    }

    isFailure() {
        return !this.success;
    }
}