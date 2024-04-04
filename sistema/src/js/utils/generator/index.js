import { v4 as uuid } from 'uuid'

export class Generator {
  static id() {
    return uuid()
  }
}