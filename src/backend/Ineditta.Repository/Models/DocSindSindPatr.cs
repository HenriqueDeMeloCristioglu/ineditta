using System;
using System.Collections.Generic;

namespace Ineditta.Repository.Models;

public partial class DocSindSindPatr
{
    public int Id { get; set; }

    public int? SindPatrIdSindp { get; set; }

    public int? DocSindIdDoc { get; set; }
}
