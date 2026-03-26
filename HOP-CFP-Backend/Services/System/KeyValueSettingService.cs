using HOP_CFP_Backend.Library.Models;
using HOP_CFP_Backend.Library.Utility;
using HOP_CFP_Backend.Utility;
using HOP_CFP_Backend.ViewModels;

namespace HOP_CFP_Backend.Services
{
    public class KeyValueSettingService : _ModelService<KeyValueSetting, KeyValueSettingModel>
    {
        public KeyValueSettingService(BaseServiceArgument argument) : base(argument) { }

        public async Task<IEnumerable<KeyValueSettingModel>> GetModelList(KeyValueSettingType type, string group)
        {
            string sql = Sql(subWhere: $"AND [Type] = @Type AND [Group] = @Group");
            IEnumerable<KeyValueSettingModel> list = await QueryAsync<KeyValueSettingModel>(sql, new { Type = type, Group = group });
            return await SetModelList(list);
        }

        public async Task<IEnumerable<KeyValueSettingModel>> GetMTMModelList(Guid sourceId, KeyValueSettingType type, string group, int? relationType = null)
        {
            string subWhere = "AND [Type] = @Type AND [Group] = @Group";
            if (relationType.HasValue)
                subWhere += "AND RelationType = @RelationType";

            string sql = SqlMTMDataList(subWhere);
            IEnumerable<KeyValueSettingModel> list = await QueryAsync<KeyValueSettingModel>(sql, new
            {
                SourceId = sourceId,
                TargetTable = _tableName,
                RelationType = relationType,
                Type = type,
                Group = group
            });
            return await SetModelList(list);
        }

    }
}
