using System;
using System.Collections.Generic;

namespace EcommercePos.Domain.Entities;

public partial class AttributeOptionMediaBlob
{
    public Guid MediaId { get; set; }

    public byte[] Data { get; set; } = null!;

    public byte[]? SwatchData { get; set; }

    public byte[]? WebpData { get; set; }

    public virtual AttributeOptionMedia Media { get; set; } = null!;
}
