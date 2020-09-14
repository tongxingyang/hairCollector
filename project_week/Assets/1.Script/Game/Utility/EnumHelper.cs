using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// -< 정의된 enum을 원하는 타입으로 변환하여 리턴한다.
public class EnumHelper
{
	// -< 해당 Enum의 리스트들을 넘겨준다.
	static public T[] getEnumType<T>()
	{	
		T[] t= ( T[] ) Enum.GetValues ( typeof( T ) );
		
		return t;
	}
	
	// -< 해당 Enum의 리스트들을 뽑아서 스트링 배열로 넘겨준다.
	static public string[] EnumsToString<T>()
	{
		T[] t = getEnumType<T>();
		List< string > typelist = new List<string>();
		for( int i= 0; i< t.Length; ++i )
			typelist.Add( t[ i ].ToString () );
		
		return typelist.ToArray();
	}

    // -< 해당 Enum값을 string으로 반환
	static public string EnumToString<T>( T t )
	{
		return t.ToString ();
	}

    // -< string을 해당 Enum타입으로 변환
    // -< 해당 타입으로 변환가능하면 변환하고 그렇지 않다면 로그를 남길 수 있도록 하기 위해서 static함수를 작성하였다.
    static public T StringToEnum< T >( string str ) where T : new()
	{
		T t = new T();
		try
        {
			t = (T)Enum.Parse( typeof( T ), str );			
		}
		catch ( ArgumentException )
		{
			Debug.Log ( str + " is the not defined by the " + typeof( T ).ToString()
                                + " enumerated type.!!!" );
		}
		return t;
	}

	// -< 해당 enum값의 전체 개수를 리턴한다.
	static public int Length<T>()
	{
		return Enum.GetValues ( typeof( T ) ).Length;
	}
}

