import { JfaseInputType } from '../../../application/jfase/constants/jfsae-input-type'
import '../../util'
import {
  createJGroup,
  createJInputDate,
  createJInputText,
  createJInputTextArea,
  createJLabel,
  createJRadioBox,
  createJSelect,
  createJTitle
} from './jform'

export function generateJfaseOptions({ text, options, id, className = '' }) {
  return [
    {
      type: JfaseInputType.Text,
      content: createJGroup({
        children: ``
          + createJLabel({ text, id })
          + createJInputText({ id }) +
          ``,
        className
      })
    },
    {
      type: JfaseInputType.TextArea,
      content: createJGroup({
        children: ``
          + createJLabel({ text, id })
          + createJInputTextArea({ id }) +
          ``,
        className
      })
    },
    {
      type: JfaseInputType.Radio,
      content: createJGroup({
        children: ``
          + createJTitle(text)
          + createJRadioBox({ id, name: id, value: 'Sim' })
          + createJRadioBox({ id: `${id}-2`, name: id, value: 'Não' }) + `
        `,
        className
      })
    },
    {
      type: JfaseInputType.Select,
      content: createJGroup({
        children: ``
          + createJLabel({ text, id })
          + createJSelect({ id, options }) +
          ``,
        className
      })
    },
    {
      type: JfaseInputType.SelectMultiple,
      content: createJGroup({
        children: ``
          + createJLabel({ text, id })
          + createJSelect({ id, options, className: 'select2', multiple: true }) +
          ``,
        className
      })
    },
    {
      type: JfaseInputType.Date,
      content: createJGroup({
        children: ``
          + createJLabel({ text, id })
          + createJInputDate({ id }) +
          ``,
        className
      })
    }
  ]
}