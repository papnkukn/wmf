using System.IO;
using System.Text;

namespace Oxage.Wmf.Objects
{
	public abstract class WmfBinaryObject : IBinaryRecord
	{
		#region IBinaryRecord Members

		public abstract void Read(BinaryReader reader);

		public abstract void Write(BinaryWriter writer);

		public string Dump()
		{
			var builder = new StringBuilder();
			Dump(builder);
			return builder.ToString();
		}

		#endregion

		public virtual void Dump(StringBuilder builder)
		{
			builder.AppendFormat("\t== {0} ==", this.GetType().Name).AppendLine();
			//Other fields should be added in overridden method
		}
	}
}
