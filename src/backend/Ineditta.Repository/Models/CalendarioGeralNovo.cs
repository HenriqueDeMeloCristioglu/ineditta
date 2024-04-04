using System;
using System.Collections.Generic;

namespace Ineditta.Repository.Models;

public partial class CalendarioGeralNovo
{
    public int Id { get; set; }

    public int? DocSindIdDoc { get; set; }

    public int? ClausulaGeralIdClau { get; set; }

    public int? AcompanhamentoCctId { get; set; }

    public int? SindDirpatroId { get; set; }

    public int? SindDirempId { get; set; }
}
