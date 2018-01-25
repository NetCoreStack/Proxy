namespace NetCoreStack.Proxy.Internal
{
    //internal static class QueryStringResolver
    //{
    //    internal static Uri Parse(UriBuilder uriBuilder, IDictionary<string, object> argsDic)
    //    {
    //        var referenceArgs = new Dictionary<string, object>();
    //        var keys = new List<string>(argsDic.Keys);

    //        foreach (string key in keys)
    //        {
    //            if (argsDic[key] == null)
    //                continue;

    //            var type = argsDic[key].GetType();

    //            if (typeof(string) == type || type.IsPrimitive())
    //                continue;

    //            referenceArgs.Add(key, argsDic[key]);

    //            // remove reference type key
    //            argsDic.Remove(key);
    //        }

    //        uriBuilder = new UriBuilder(uriBuilder.Uri.ToQueryString(argsDic));

    //        // Resolve reference type args
    //        keys = new List<string>(referenceArgs.Keys);            
    //        foreach (string key in keys)
    //        {
    //            var type = referenceArgs[key].GetType();

    //            if (typeof(IEnumerable<Column>).IsAssignableFrom(type))
    //            {
    //                List<string> values = new List<string>();
    //                var data = referenceArgs[key] as IEnumerable<Column>;
    //                var index = 0;
    //                foreach (var item in data)
    //                {
    //                    values.Add($"{key}[{index}][{nameof(Column.Data)}][_]={item.Data}");
    //                    values.Add($"{key}[{index}][{nameof(Column.Data)}][{nameof(Column.Meta)}]={item.Meta}");
    //                    values.Add($"{key}[{index}][{nameof(Column.Data)}][{nameof(Column.Composer)}]={item.Composer}");
    //                    values.Add($"{key}[{index}][name]=");
    //                    values.Add($"{key}[{index}][{nameof(Column.Searchable)}]={item.Searchable}");
    //                    values.Add($"{key}[{index}][{nameof(Column.Orderable)}]={item.Orderable}");
    //                    values.Add($"{key}[{index}][{nameof(Column.Search)}][value]={item.Search?.Value}");
    //                    values.Add($"{key}[{index}][{nameof(Column.Search)}][regex]={item.Search?.IsRegex}");

    //                    index++;
    //                }

    //                uriBuilder.Query += "&" + string.Join("&", values);
    //                continue;
    //            }

    //            if (typeof(IEnumerable<OrderDescriptor>).IsAssignableFrom(type))
    //            {
    //                List<string> values = new List<string>();
    //                var data = referenceArgs[key] as IEnumerable<OrderDescriptor>;
    //                var index = 0;
    //                foreach (var item in data)
    //                {
    //                    values.Add($"{key}[{index}][{nameof(Column)}]={item.ColumnIndex}");
    //                    values.Add($"{key}[{index}][dir]={item.Direction}");
    //                    index++;
    //                }

    //                uriBuilder.Query += "&" + string.Join("&", values);
    //                continue;
    //            }

    //            if (typeof(IEnumerable).IsAssignableFrom(type))
    //            {
    //                List<string> values = new List<string>();
    //                var data = referenceArgs[key] as IEnumerable;
    //                var index = 0;
    //                foreach (var item in data)
    //                {
    //                    values.Add($"{key}[{index}]={item}");
    //                    index++;
    //                }

    //                uriBuilder.Query += "&" + string.Join("&", values);
    //                continue;
    //            }
    //        }

    //        return uriBuilder.Uri;
    //    }
    //}
}
