import { LinkFormatterType } from "./link-formatter-type"

export class LinkFormatter {
  static midiasSociais(string, tipo) {
    if (string == null) return ""

    if (string.includes(".com")) {
      if (string.includes("http")) {
        return string
      } else {
        return `https://${string}`
      }
    }
  
    if (tipo === LinkFormatterType.Site) {
      if (string.includes("http")) {
        return string
      } else {
        return `https://${string}`
      }
    }
  
    if (tipo === LinkFormatterType.Twitter) {
      return `https://twitter.com/${string}`
    }

    if (tipo === LinkFormatterType.Intagram) {
      return `https://instagram.com/${string}`
    }

    if (tipo === LinkFormatterType.Facebook) {
      return `https://facebook.com/${string}`
    }
  }
}