using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Optimization;

namespace pnacpacam.App_Start
{
    public class ProperStyleBundle : StyleBundle
    {
        public override IBundleOrderer Orderer
        {
            get { return new NonOrderingBundleOrderer(); }
            set { throw new Exception("Unable to override Non-Ordered bundler"); }
        }

        public ProperStyleBundle(string virtualPath) : base(virtualPath) { }

        public ProperStyleBundle(string virtualPath, string cdnPath) : base(virtualPath, cdnPath) { }

        public override Bundle Include(params string[] virtualPaths)
        {
            foreach (var virtualPath in virtualPaths)
            {
                this.Include(virtualPath);
            }
            return this;
        }

        public override Bundle Include(string virtualPath, params IItemTransform[] transforms)
        {
            var realPath = System.Web.Hosting.HostingEnvironment.MapPath(virtualPath);
            if (!File.Exists(realPath))
            {
                throw new FileNotFoundException("Virtual path not found: " + virtualPath);
            }
            var trans = new List<IItemTransform>(transforms).Union(new[] { new ProperCssRewriteUrlTransform(virtualPath) }).ToArray();
            return base.Include(virtualPath, trans);
        }

        // This provides files in the same order as they have been added. 
        private class NonOrderingBundleOrderer : IBundleOrderer
        {
            public IEnumerable<BundleFile> OrderFiles(BundleContext context, IEnumerable<BundleFile> files)
            {
                return files;
            }
        }

        private class ProperCssRewriteUrlTransform : IItemTransform
        {
            private readonly string _basePath;

            public ProperCssRewriteUrlTransform(string basePath)
            {
                _basePath = basePath.EndsWith("/") ? basePath : VirtualPathUtility.GetDirectory(basePath);
            }

            public string Process(string includedVirtualPath, string input)
            {
                if (includedVirtualPath == null)
                {
                    throw new ArgumentNullException("includedVirtualPath");
                }
                return ConvertUrlsToAbsolute(_basePath, input);
            }

            private static string RebaseUrlToAbsolute(string baseUrl, string url)
            {
                if (string.IsNullOrWhiteSpace(url)
                     || string.IsNullOrWhiteSpace(baseUrl)
                     || url.StartsWith("/", StringComparison.OrdinalIgnoreCase)
                     || url.StartsWith("data:", StringComparison.OrdinalIgnoreCase)
                    )
                {
                    return url;
                }
                if (!baseUrl.EndsWith("/", StringComparison.OrdinalIgnoreCase))
                {
                    baseUrl = baseUrl + "/";
                }
                return VirtualPathUtility.ToAbsolute(baseUrl + url);
            }

            private static string ConvertUrlsToAbsolute(string baseUrl, string content)
            {
                if (string.IsNullOrWhiteSpace(content))
                {
                    return content;
                }
                return new Regex("url\\(['\"]?(?<url>[^)]+?)['\"]?\\)")
                    .Replace(content, (match =>
                                       "url(" + RebaseUrlToAbsolute(baseUrl, match.Groups["url"].Value) + ")"));
            }
        }
    }
}