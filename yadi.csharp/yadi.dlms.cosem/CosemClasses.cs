using System.Collections.Generic;

/*
 * YADI (Yet Another DLMS Implementation)
 * Copyright (C) 2018 Paulo Faco (paulofaco@gmail.com)
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

/*
* YADI.CSHARP
* Ported from Java to C# by Gabriel.Habib (enggabrielhabib@gmail.com) & Enzo Furlan (enzo.furlan@live.com)
*/
namespace yadi.dlms.cosem
{
    public sealed class CosemClasses
    {
        public static readonly CosemClasses DATA = new CosemClasses("DATA", InnerEnum.DATA, 1);
        public static readonly CosemClasses REGISTER = new CosemClasses("REGISTER", InnerEnum.REGISTER, 3);
        public static readonly CosemClasses EXTENDED_REGISTER = new CosemClasses("EXTENDED_REGISTER", InnerEnum.EXTENDED_REGISTER, 4);
        public static readonly CosemClasses DEMAND_REGISTER = new CosemClasses("DEMAND_REGISTER", InnerEnum.DEMAND_REGISTER, 5);
        public static readonly CosemClasses REGISTER_ACTIVATION = new CosemClasses("REGISTER_ACTIVATION", InnerEnum.REGISTER_ACTIVATION, 6);
        public static readonly CosemClasses PROFILE_GENERIC = new CosemClasses("PROFILE_GENERIC", InnerEnum.PROFILE_GENERIC, 7);
        public static readonly CosemClasses CLOCK = new CosemClasses("CLOCK", InnerEnum.CLOCK, 8);
        public static readonly CosemClasses SCRIPT_TABLE = new CosemClasses("SCRIPT_TABLE", InnerEnum.SCRIPT_TABLE, 9);
        public static readonly CosemClasses SCHEDULE = new CosemClasses("SCHEDULE", InnerEnum.SCHEDULE, 10);
        public static readonly CosemClasses SPECIAL_DAYS_TABLE = new CosemClasses("SPECIAL_DAYS_TABLE", InnerEnum.SPECIAL_DAYS_TABLE, 11);
        public static readonly CosemClasses ASSOCIATION_SN = new CosemClasses("ASSOCIATION_SN", InnerEnum.ASSOCIATION_SN, 12);
        public static readonly CosemClasses ASSOCIATION_LN = new CosemClasses("ASSOCIATION_LN", InnerEnum.ASSOCIATION_LN, 15);
        public static readonly CosemClasses SAP_ASSIGNMENT = new CosemClasses("SAP_ASSIGNMENT", InnerEnum.SAP_ASSIGNMENT, 17);
        public static readonly CosemClasses IMAGE_TRANSFER = new CosemClasses("IMAGE_TRANSFER", InnerEnum.IMAGE_TRANSFER, 18);
        public static readonly CosemClasses IEC_LOCAL_PORT_SETUP = new CosemClasses("IEC_LOCAL_PORT_SETUP", InnerEnum.IEC_LOCAL_PORT_SETUP, 19);
        public static readonly CosemClasses ACTIVITY_CALENDAR = new CosemClasses("ACTIVITY_CALENDAR", InnerEnum.ACTIVITY_CALENDAR, 20);
        public static readonly CosemClasses REGISTER_MONITOR = new CosemClasses("REGISTER_MONITOR", InnerEnum.REGISTER_MONITOR, 21);
        public static readonly CosemClasses SINGLE_ACTION_SCHEDULE = new CosemClasses("SINGLE_ACTION_SCHEDULE", InnerEnum.SINGLE_ACTION_SCHEDULE, 22);
        public static readonly CosemClasses IEC_HDLC_SETUP = new CosemClasses("IEC_HDLC_SETUP", InnerEnum.IEC_HDLC_SETUP, 23);
        public static readonly CosemClasses UTILITY_TABLES = new CosemClasses("UTILITY_TABLES", InnerEnum.UTILITY_TABLES, 26);
        public static readonly CosemClasses DATA_PROTECTION = new CosemClasses("DATA_PROTECTION", InnerEnum.DATA_PROTECTION, 30);
        public static readonly CosemClasses PUSH_SETUP = new CosemClasses("PUSH_SETUP", InnerEnum.PUSH_SETUP, 40);
        public static readonly CosemClasses REGISTER_TABLE = new CosemClasses("REGISTER_TABLE", InnerEnum.REGISTER_TABLE, 61);
        public static readonly CosemClasses COMPACT_DATA = new CosemClasses("COMPACT_DATA", InnerEnum.COMPACT_DATA, 62);
        public static readonly CosemClasses STATUS_MAPPING = new CosemClasses("STATUS_MAPPING", InnerEnum.STATUS_MAPPING, 63);
        public static readonly CosemClasses SECURITY_SETUP = new CosemClasses("SECURITY_SETUP", InnerEnum.SECURITY_SETUP, 64);
        public static readonly CosemClasses PARAMETER_MONITOR = new CosemClasses("PARAMETER_MONITOR", InnerEnum.PARAMETER_MONITOR, 65);
        public static readonly CosemClasses SENSOR_MANAGER = new CosemClasses("SENSOR_MANAGER", InnerEnum.SENSOR_MANAGER, 67);
        public static readonly CosemClasses ARBITRATOR = new CosemClasses("ARBITRATOR", InnerEnum.ARBITRATOR, 68);
        public static readonly CosemClasses DISCONNECT_CONTROL = new CosemClasses("DISCONNECT_CONTROL", InnerEnum.DISCONNECT_CONTROL, 70);
        public static readonly CosemClasses LIMITER = new CosemClasses("LIMITER", InnerEnum.LIMITER, 71);
        public static readonly CosemClasses ACCOUNT = new CosemClasses("ACCOUNT", InnerEnum.ACCOUNT, 111);
        public static readonly CosemClasses CREDIT = new CosemClasses("CREDIT", InnerEnum.CREDIT, 112);
        public static readonly CosemClasses CHARGE = new CosemClasses("CHARGE", InnerEnum.CHARGE, 113);
        public static readonly CosemClasses TOKEN_GATEWAY = new CosemClasses("TOKEN_GATEWAY", InnerEnum.TOKEN_GATEWAY, 115);
        public static readonly CosemClasses FUNCTION_CONTROL = new CosemClasses("FUNCTION_CONTROL", InnerEnum.FUNCTION_CONTROL, 122);
        public static readonly CosemClasses ARRAY_MANAGER = new CosemClasses("ARRAY_MANAGER", InnerEnum.ARRAY_MANAGER, 123);

        private static readonly IList<CosemClasses> valueList = new List<CosemClasses>();

        static CosemClasses()
        {
            valueList.Add(DATA);
            valueList.Add(REGISTER);
            valueList.Add(EXTENDED_REGISTER);
            valueList.Add(DEMAND_REGISTER);
            valueList.Add(REGISTER_ACTIVATION);
            valueList.Add(PROFILE_GENERIC);
            valueList.Add(CLOCK);
            valueList.Add(SCRIPT_TABLE);
            valueList.Add(SCHEDULE);
            valueList.Add(SPECIAL_DAYS_TABLE);
            valueList.Add(ASSOCIATION_SN);
            valueList.Add(ASSOCIATION_LN);
            valueList.Add(SAP_ASSIGNMENT);
            valueList.Add(IMAGE_TRANSFER);
            valueList.Add(IEC_LOCAL_PORT_SETUP);
            valueList.Add(ACTIVITY_CALENDAR);
            valueList.Add(REGISTER_MONITOR);
            valueList.Add(SINGLE_ACTION_SCHEDULE);
            valueList.Add(IEC_HDLC_SETUP);
            valueList.Add(UTILITY_TABLES);
            valueList.Add(DATA_PROTECTION);
            valueList.Add(PUSH_SETUP);
            valueList.Add(REGISTER_TABLE);
            valueList.Add(COMPACT_DATA);
            valueList.Add(STATUS_MAPPING);
            valueList.Add(SECURITY_SETUP);
            valueList.Add(PARAMETER_MONITOR);
            valueList.Add(SENSOR_MANAGER);
            valueList.Add(ARBITRATOR);
            valueList.Add(DISCONNECT_CONTROL);
            valueList.Add(LIMITER);
            valueList.Add(ACCOUNT);
            valueList.Add(CREDIT);
            valueList.Add(CHARGE);
            valueList.Add(TOKEN_GATEWAY);
            valueList.Add(FUNCTION_CONTROL);
            valueList.Add(ARRAY_MANAGER);
        }

        public enum InnerEnum
        {
            DATA,
            REGISTER,
            EXTENDED_REGISTER,
            DEMAND_REGISTER,
            REGISTER_ACTIVATION,
            PROFILE_GENERIC,
            CLOCK,
            SCRIPT_TABLE,
            SCHEDULE,
            SPECIAL_DAYS_TABLE,
            ASSOCIATION_SN,
            ASSOCIATION_LN,
            SAP_ASSIGNMENT,
            IMAGE_TRANSFER,
            IEC_LOCAL_PORT_SETUP,
            ACTIVITY_CALENDAR,
            REGISTER_MONITOR,
            SINGLE_ACTION_SCHEDULE,
            IEC_HDLC_SETUP,
            UTILITY_TABLES,
            DATA_PROTECTION,
            PUSH_SETUP,
            REGISTER_TABLE,
            COMPACT_DATA,
            STATUS_MAPPING,
            SECURITY_SETUP,
            PARAMETER_MONITOR,
            SENSOR_MANAGER,
            ARBITRATOR,
            DISCONNECT_CONTROL,
            LIMITER,
            ACCOUNT,
            CREDIT,
            CHARGE,
            TOKEN_GATEWAY,
            FUNCTION_CONTROL,
            ARRAY_MANAGER
        }

        public readonly InnerEnum innerEnumValue;
        private readonly string nameValue;
        private readonly int ordinalValue;
        private static int nextOrdinal = 0;

        public readonly int id;

        private CosemClasses(string name, InnerEnum innerEnum, int id)
        {
            this.id = id;

            nameValue = name;
            ordinalValue = nextOrdinal++;
            innerEnumValue = innerEnum;
        }

        public static IList<CosemClasses> values()
        {
            return valueList;
        }

        public int ordinal()
        {
            return ordinalValue;
        }

        public override string ToString()
        {
            return nameValue;
        }

        public static CosemClasses valueOf(string name)
        {
            foreach (CosemClasses enumInstance in CosemClasses.valueList)
            {
                if (enumInstance.nameValue == name)
                {
                    return enumInstance;
                }
            }
            throw new System.ArgumentException(name);
        }
    }
}
