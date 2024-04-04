using Humanizer;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System.Text.RegularExpressions;

namespace Ineditta.BuildingBlocks.Core.Web.API.Convetions
{
    public class PluralizeKebabCaseControllerNamesConvention : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
#pragma warning disable SYSLIB1045 // Convert to 'GeneratedRegexAttribute'.
            var words = Regex.Split(controller.ControllerName, @"(?<!^)(?=[A-Z])", RegexOptions.None, TimeSpan.FromSeconds(60));
#pragma warning restore SYSLIB1045 // Convert to 'GeneratedRegexAttribute'.

            // Pluralize each word
            for (int i = 0; i < words.Length; i++)
            {
                words[i] = words[i].Pluralize();
            }

            // Join the words back and convert to camelCase
            var camelCaseName = string.Concat(words).Underscore();

            // Convert to kebab-case and lowercase
            var kebabCaseName = camelCaseName.Dasherize().ToLowerInvariant();

            controller.ControllerName = kebabCaseName;
        }
    }
}
