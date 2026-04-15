using System;
using System.Collections.Generic;
using EcommercePos.Domain.Common;

namespace EcommercePos.Domain.Entities;

public partial class Blogs : AuditableEntity<Guid>
{
    public Guid CategoryId { get; set; }

    public Guid? AuthorId { get; set; }

    public string Title { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string Content { get; set; } = null!;

    public string? ShortDescription { get; set; }

    public string? ThumbnailUrl { get; set; }

    public string? BannerUrl { get; set; }

    public bool IsPublished { get; set; }

    public DateTime? PublishedAt { get; set; }

    public int ViewCount { get; set; }

    public string? MetaTitle { get; set; }

    public string? MetaDescription { get; set; }
    public virtual Users? Author { get; set; }

    public virtual ICollection<BlogComments> BlogComments { get; set; } = new List<BlogComments>();

    public virtual BlogCategories Category { get; set; } = null!;

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual Users? UpdatedByNavigation { get; set; }

    public virtual ICollection<BlogTags> BlogTag { get; set; } = new List<BlogTags>();
}
