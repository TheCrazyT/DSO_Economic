using Microsoft.Win32;
using System;
using System.Runtime.InteropServices;

namespace Protocol
{

	[ComVisible(false)] public interface IComRegister
	{
		void Register(Type t);
		void Unregister(Type t);
	}

	/// <summary>
	/// Registers an Asyncrhonous Pluggable Protocol of the form {Name}:Url
	/// Your class needs to provide two methods of the following types or derive from ProtocolBase.
	/// [ AsyncProtocol(Name="echo", Description="Returns the URL of the protocol as HTML content.") ]
	/// public MyComClass 
	/// {
	///		[ComRegisterFunction] 
	///		private static void RegisterProtocol(Type t)
	///		{
	///			ProtocolSupport.RegisterProtocol(t);
	///		}
	///		[ComUnregisterFunction] 
	///		private static void UnregisterProtocol(Type t)
	///		{
	///			ProtocolSupport.UnregisterProtocol(t);
	///		}
	///		
	///		//MyClass Methods...
	///	}
	/// </summary>
	[ComVisible(false), AttributeUsage(AttributeTargets.Class, AllowMultiple=true) ] 
	public class AsyncProtocolAttribute : Attribute, IComRegister
	{
		public string Name;
		public string Description;

		public void Register(Type t)
		{
			RegistryKey ProtocolKey = Registry.ClassesRoot.CreateSubKey(RegistryPath);
			ProtocolKey.SetValue(null, Description);
			ProtocolKey.SetValue("CLSID", "{" + ProtocolSupport.GetGuid(t) + "}");
			Console.WriteLine("Registered Protocol:" + Name);
		}

		public void Unregister(Type t)
		{
			try 
			{
				Registry.ClassesRoot.DeleteSubKeyTree(RegistryPath);
				Console.WriteLine("UnRegistered Protocol:" + Name);
			} 
			catch (ArgumentException)	{ /*sink this exception because we don't care if this key doesn't exist */ }
		}

		protected string RegistryPath
		{
			get { return @"PROTOCOLS\Handler\" + Name; }
		}

	}

	[ComVisible(false), AttributeUsage(AttributeTargets.Class, AllowMultiple=true) ] 
	public class ContextHandlerAttribute : Attribute, IComRegister
	{
		#region IComRegister Members

		public string Key;
		public string Name;
		public string Description;

		public void Register(Type t)
		{
			RegistryKey ProtocolKey = Registry.ClassesRoot.CreateSubKey(RegistryPath);
			ProtocolKey.SetValue(null, "{" + ProtocolSupport.GetGuid(t) + "}");
			Console.WriteLine("Registered ContextHandler:" + Key + "|" + Name);
		}

		public void Unregister(Type t)
		{
			try 
			{
				Registry.ClassesRoot.DeleteSubKeyTree(RegistryPath);
				Console.WriteLine("UnRegistered ContextHandler:" + Key + "|" + Name);
			} 
			catch (ArgumentException)	{ /*sink this exception because we don't care if this key doesn't exist */ }
		}

		protected string RegistryPath
		{
			get { return Key + @"\shellex\ContextMenuHandlers\" + Name; }
		}

		#endregion
	}
		
	/// <summary>
	/// This class provides the support for protocol registration based on the AsyncProtocolAttribute class.
	/// </summary>
	[ComVisible(false)] public class ProtocolSupport
	{
		public static void RegisterProtocol(Type t)
		{
			IComRegister[] Protocols = GetAttributes(t);
			if (Protocols == null || Protocols.Length == 0)
				return;
			foreach(IComRegister Protocol in Protocols)
				Protocol.Register(t);
		}

		public static void UnregisterProtocol(Type t)
		{
			IComRegister[] Protocols = GetAttributes(t);
			if (Protocols == null || Protocols.Length == 0)
				return;
			foreach(IComRegister Protocol in Protocols)
				Protocol.Unregister(t);
		}

		public static IComRegister[] GetAttributes(Type t)
		{
			return (IComRegister[])t.GetCustomAttributes(typeof(IComRegister), false);
		}

		public static string GetGuid(Type t)
		{
			object[] Guids = t.GetCustomAttributes(typeof(GuidAttribute), false);
			if (Guids == null || Guids.Length == 0)
				throw new Exception("All Types marked with the ProtocolAttribute must be marked with the GuidAttribute.");
			return ((GuidAttribute)Guids[0]).Value;
		}

	}
}
