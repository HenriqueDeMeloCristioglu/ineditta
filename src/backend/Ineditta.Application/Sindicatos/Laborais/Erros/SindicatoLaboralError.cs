namespace Ineditta.Application.Sindicatos.Laborais.Erros
{
    public static class SindicatoLaboralError
    {
        public static string CampoVazio(string campo)
        {
            return "Campo " + campo + "vazio";
        }

        public static string CampoInvalido(string campo)
        {
            return "Campo " + campo + "inválido";
        }
    }
}
