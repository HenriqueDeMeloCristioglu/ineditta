export default class PageWrapper {
    constructor(bytes, fileName, type) {
        this.bytes = bytes
        this.fileName = fileName
        this.type = type

        this.init()
    }

    init() {
        const file = new Blob([this.bytes], { type: this.type })

        const fileURL = window.URL.createObjectURL(file)
        const fileLink = document.createElement('a')

        fileLink.href = fileURL
        fileLink.setAttribute('download', this.fileName)
        document.body.appendChild(fileLink)

        fileLink.click()

        window.URL.revokeObjectURL(fileURL)
        fileLink.remove()
    }

    static downloadExcel(bytes, fileName) {
        new PageWrapper(bytes, fileName, 'application/vnd.ms-excel')
    }

    static download(bytes, fileName, type) {
        new PageWrapper(bytes, fileName, type)
    }

    static preview(bytes, type) {
        const blobFile = new Blob([bytes], { type })

        const urlFile = URL.createObjectURL(blobFile)

        window.open(urlFile, '_blank')

        URL.revokeObjectURL(urlFile)
    }
}