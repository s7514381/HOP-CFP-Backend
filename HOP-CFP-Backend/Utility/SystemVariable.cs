using System;
using System.Threading.Tasks;
using HOP_CFP_Backend.Library.Models.System;
using HOP_CFP_Backend.Services;

namespace HOP_CFP_Backend.Utility
{
    public static class SystemVariable
    {
        public static bool EnableTestMode { get; private set; }


        private static bool _testModeOn;
        public static bool TestModeOn
        {
            get
            {
                return EnableTestMode && _testModeOn;
            }
            private set
            {
                _testModeOn = value;
            }
        }

        private static TimeSpan _delayTimeSpan;

        static SystemVariable()
        {
            EnableTestMode = false;
            TestModeOn = false;
            _delayTimeSpan = TimeSpan.Zero;
        }

        public static async Task InitAsync(bool enableTestMode, SysConfigService sysConfigService)
        {
            var testModeOnAsync = sysConfigService.GetAsync("test.TestModeOn");
            var timeSpanAsync = sysConfigService.GetAsync("test.TimeSpan");
            EnableTestMode = enableTestMode;
            TestModeOn = bool.Parse((await testModeOnAsync).Value);
            _delayTimeSpan = TimeSpan.FromTicks(long.Parse((await timeSpanAsync).Value));
        }

        public static DateTime Now
        {
            get
            {
                if (EnableTestMode && TestModeOn)
                {
                    return DateTime.Now + _delayTimeSpan;
                }

                return DateTime.Now;
            }
        }

        public static DateTime Today
        {
            get
            {
                return Now.Date;
            }
        }

        public static async Task SetStatusAsync(bool on, SysConfigService sysConfigService)
        {
            if (on != TestModeOn)
            {
                TestModeOn = on;
                await sysConfigService.UpdateAsync(new SysConfig
                {
                    Id = Guid.NewGuid(),
                    Value = on.ToString(),
                    Note = "測試模式開啟狀態",
                    TypeName = "TestMode"
                });
            }
        }

        public static async Task SetNowAsync(DateTime now, SysConfigService sysConfigService)
        {
            if (now == default)
                throw new ArgumentOutOfRangeException(nameof(now));
            _delayTimeSpan = now - DateTime.Now;
            await sysConfigService.UpdateAsync(new SysConfig
            {
                Id = Guid.NewGuid(),
                Value = _delayTimeSpan.Ticks.ToString(),
                Note = "測試模式時間位移量",
                TypeName = "TestMode"
            });
        }
    }
}
