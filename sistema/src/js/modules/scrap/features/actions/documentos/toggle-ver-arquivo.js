import $ from 'jquery'

export async function toggleVerArquivo(context) {
  const { datas } = context
  const { verDocumento, documentoPath } = datas

  const embed = $('#embed_pdf')
  
  datas.verDocumento = !verDocumento
  
  if (datas.verDocumento) {
    $("#ver_documento_ctn").show()

    return embed.attr('src', documentoPath)
  }

  embed.attr('src', '')
  $("#ver_documento_ctn").hide()
}