using System;
using System.Collections.Generic;

namespace PictureManager.Entities;

public partial class Picture
{
    public int Id { get; set; }

    public string? Path { get; set; }

    public byte[]? Data { get; set; }
}
