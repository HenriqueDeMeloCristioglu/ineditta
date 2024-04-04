
Date.prototype.subtractMonths = function (months = 0) {
    if (!months || months <= 0) {
        return this;
    }
    for (let i = months; i > 0; i--) {
        if (this.getMonth() == 0) {
            this.setFullYear(this.getFullYear() - 1);
            this.setMonth(11);
            continue;
        }

        this.setMonth(this.getMonth() - 1);
    }

    return this;
};

Date.prototype.addMonths = function (months = 0) {
    if (!months || months <= 0) {
        return this;
    }

    for (let i = 0; i < months; i++) {
        if (this.getMonth() == 11) {
            this.setFullYear(this.getFullYear() + 1);
            this.setMonth(0);
            continue;
        }

        this.setMonth(this.getMonth() + 1);
    }

    return this;
};

Date.prototype.getMonthsOfYear = function () {
    const result = [];
    for (let month = 0; month < 12; month++) {
        const date = new Date(2000, month, 1);

        const monthName = date.toLocaleString('pt-BR', { month: 'long' });

        const name = monthName.charAt(0).toUpperCase() + monthName.slice(1);

        const item = {
            month: date.getMonth() + 1,
            name: name,
            shortName: date.toLocaleString('pt-BR', { month: 'short' })
        };

        result.push(item);
    }

    return result;
};