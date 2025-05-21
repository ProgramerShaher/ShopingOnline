
namespace ShopingOnline.Services
{
    using PayPalCheckoutSdk.Core;
    using PayPalHttp;

    public static class PayPalClient
    {
        public static PayPalEnvironment Environment => new SandboxEnvironment(
            "AcpN_tkMxZekkFrlZbWJhiFvzP00C-007hwQrsQM16IbCu6Gm0f8SlNfu2JhsVRZP75tGWBoY4ABWv6m",     // ضع هنا PayPal Client ID
            "EIBMTrmWklKiV7-DxZj85cX35nEdw4_ZO_hX9P_naIEysa2-xVuIIWjcolYQyaoY1Q_UkiJUOtPjTN6E"  // ضع هنا PayPal Client Secret
        );

        public static HttpClient Client() => new PayPalHttpClient(Environment);
    }

}
