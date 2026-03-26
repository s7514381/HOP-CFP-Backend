using Dapper.Contrib.Extensions;
using Microsoft.EntityFrameworkCore;

namespace HOP_CFP_Backend.Library.Models
{
    [Table(nameof(KeyValueSetting))]
    public class KeyValueSetting : IdModelBase
    {
        public KeyValueSettingType? Type { get; set; }

        public string? Key { get; set; }

        public string? Value { get; set; }

        public string? Group { get; set; }

        public override void OnModelCreating(ModelBuilder modelBuilder) 
        {
            modelBuilder.Entity<KeyValueSetting>()
                 .HasIndex(b => new { b.Type, b.Group, b.Key });
        }
    }

    public enum KeyValueSettingType
    { 
        Default = 1,
    }

}
