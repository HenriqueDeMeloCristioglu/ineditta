import { UsuarioNivel } from "../../../../application/usuarios/constants/usuario-nivel"
import { UsuarioTipo } from "../../../../application/usuarios/constants/usuario-tipo"
import { obterPermissoesUsuario } from "../../../core"
import { useUsuarioService } from "../../../core/hooks"

const usuarioAdmService = useUsuarioService()

export async function configurarPermissoesPagina(context) {
  const { datas } = context

  datas.modulos = await obterPermissoesUsuario()

  datas.dadosPessoais = await usuarioAdmService.obterDadosPessoais()
  if (datas.dadosPessoais.isFailure()) return

  datas.usuario = datas.dadosPessoais.value

  datas.isIneditta = datas.usuario.nivel == UsuarioNivel.Ineditta
  datas.isGrupoEconomico = datas.usuario.nivel == UsuarioNivel.GrupoEconomico
  datas.isEstabelecimento = datas.usuario.nivel == UsuarioNivel.Estabelecimento
  datas.isEmpresa = datas.usuario.nivel == UsuarioNivel.Empresa
  datas.isEmpresa = datas.usuario.nivel == UsuarioNivel.Empresa
  datas.isCliente = datas.usuario.tipo == UsuarioTipo.Cliente ? () => true : () => false
}