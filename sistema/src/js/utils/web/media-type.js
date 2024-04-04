export class MediaType {
  static excel = {
    Accept: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
  }

  static json = {
    'Content-Type': 'application/json'
  }

  static pdf = {
    'Content-Type': 'application/json',
    Accept: 'application/pdf'
  }

  static select = {
    'Content-Type': 'application/json',
    Accept: `application/vnd.ineditta.select+json`
  }

  static dataTable = {
    'Content-Type': 'application/json',
    Accept: `application/vnd.ineditta.datatables+json`
  }

  static stream = {
    Accept: 'application/octet-stream'
  }

  static multipartFormData = {
    'Content-Type': 'multipart/form-data',
    'Accept': 'application/json'
  }
  static urlencoded = {
    "Content-Type": "application/x-www-form-urlencoded"
  }

  static requestId = (id) => {
    return {
      'X-request-ID': id
    }
  }
}