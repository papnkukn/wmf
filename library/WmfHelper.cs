using System;

namespace Oxage.Wmf
{
	public class WmfHelper
	{
		public static IBinaryRecord GetRecordByType(RecordType rt)
		{
			var types = typeof(WmfHelper).Assembly.GetTypes();

			foreach (var type in types)
			{
				if (typeof(IBinaryRecord).IsAssignableFrom(type))
				{
					var attribute = Attribute.GetCustomAttribute(type, typeof(WmfRecordAttribute)) as WmfRecordAttribute;
					if (attribute != null && attribute.Type == rt)
					{
						var record = Activator.CreateInstance(type) as IBinaryRecord;
						return record;
					}
				}
			}

			return null;
		}
	}
}
