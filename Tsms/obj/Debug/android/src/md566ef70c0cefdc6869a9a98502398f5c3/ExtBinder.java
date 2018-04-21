package md566ef70c0cefdc6869a9a98502398f5c3;


public class ExtBinder
	extends android.os.Binder
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("Tsms.ExtBinder, Tsms, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", ExtBinder.class, __md_methods);
	}


	public ExtBinder () throws java.lang.Throwable
	{
		super ();
		if (getClass () == ExtBinder.class)
			mono.android.TypeManager.Activate ("Tsms.ExtBinder, Tsms, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
