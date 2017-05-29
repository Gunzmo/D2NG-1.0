using D2NG_Final.Pointers;
using D2NG_Final.Pointers.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhiteMagic;
using WhiteMagic.Modules;
using WhiteMagic.WinAPI;
using WhiteMagic.WinAPI.Structures;

namespace D2NG_1._0.Pointers
{
    class Infravision : HardwareBreakPoint
    {
        public Infravision() : base(new IntPtr(0xD4C31), BreakpointCondition.Code, 1) { }
    /*
        public override bool HandleException(ref CONTEXT ctx, ProcessDebugger pd)
        {
            // Esi - pUnit
            var hide = false;

            var pUnit = ctx.Esi;
            if (pUnit == 0)
                hide = true;
            else if (true)
            {
                var unit = pd.Read<UnitAny>(new IntPtr(pUnit));
                switch ((UnitType)unit.dwType)
                {
                    case UnitType.Monster:
                        {
                            hide = true;
                            break;
                        }
                    case UnitType.Item:
                        {
                            /*
                            if (!Game.Settings.Infravision.HideItems.IsEnabled())
                                break;

                            var itemInfo = ItemStorage.GetInfo(unit.dwTxtFileNo);
                            if (itemInfo != null)
                            {
                                var itemData = pd.Read<ItemData>(unit.pItemData);
                                var pTxt = Game.GetItemText(unit.dwTxtFileNo);
                                var txt = pd.Read<ItemTxt>(pTxt);

                                var sock = Game.GetItemSockets(pUnit, unit.dwUnitId);
                                var configEntries = Game.ItemProcessingSettings.GetMatches(itemInfo, sock,
                                    (itemData.dwFlags & 0x400000) != 0, (ItemQuality)itemData.dwQuality).Where(it => it.Hide);
                                if (configEntries.Count() != 0)
                                    hide = true;
                                   */
                        }
                        break;
                }
            }
            ctx.Eax = hide ? 1u : 0u;
            ctx.Eip += 0x77;
            return true;
        }
    */
    }
}
