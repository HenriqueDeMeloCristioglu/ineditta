Array.prototype.distinct = function () {
    return this.filter((item, index, arr) => arr.indexOf(item) === index);
};

Array.prototype.pushArray = function (arr) {
    this.push.apply(this, arr)
}
