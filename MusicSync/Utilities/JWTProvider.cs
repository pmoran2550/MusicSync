using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Configuration;
using System.Windows.Input;

namespace MusicSync.Utilities
{
    public class JWTProvider
    {

        public string CreateJwt()
        {
            var key = GetECDsaKey("C:\\Development\\keys\\AuthKey_86S88FMZ8T.p8");
            var securityKey = new ECDsaSecurityKey(key) { KeyId = "86S88FMZ8T" };
            var descriptor = new SecurityTokenDescriptor
            {
                IssuedAt = DateTime.Now,
                Issuer = "632DRQZ7LL",
                SigningCredentials = new SigningCredentials(securityKey, "ES256")
            };

            var handler = new JwtSecurityTokenHandler();
            var encodedToken = handler.CreateEncodedJwt(descriptor);
            return encodedToken;
        }


        private static ECDsaCng GetECDsaKey(string privateKeyPath)
        {
            var privateKeyContent = File.ReadAllText(privateKeyPath);
            var privateKeyList = privateKeyContent.Split('\n').ToList();

            // key contains newlines. Only take base 64 encoded string exclude cert header and footer
            var privateKey = privateKeyList
                .Where((s, i) => i != 0 && i != privateKeyList.Count - 1)
                .Aggregate((agg, s) => agg + s);

            return new ECDsaCng(CngKey.Import(Convert.FromBase64String(privateKey), CngKeyBlobFormat.Pkcs8PrivateBlob));
        }

        //    public string GenerateToken()
        //    {
        //        var algorithm = "ES256";

        //        /* 
        //* The Key ID of your MusicKit Private key:
        //* https://developer.apple.com/account/ios/authkey/
        //* For more information, go to Apple Music API Reference:
        //* https://developer.apple.com/library/content/documentation/NetworkingInternetWeb/Conceptual/AppleMusicWebServicesReference/SetUpWebServices.html#//apple_ref/doc/uid/TP40017625-CH2-SW1
        //*/
        //        var keyId = "86S88FMZ8T";
        //        Console.WriteLine($"Your MusicKit Private Key ID: {keyId}");

        //        // Your Team ID from your Apple Developer Account:
        //        // https://developer.apple.com/account/#/membership/
        //        var teamId = "632DRQZ7LL";
        //        Console.WriteLine($"Your Apple Team ID: {keyId}");

        //        var utcNow = DateTime.UtcNow;
        //        var epoch = new DateTime(1970, 1, 1);
        //        var epochNow = (int)utcNow.Subtract(epoch).TotalSeconds;
        //        var utcExpire = utcNow.AddMonths(6);
        //        var epochExpire = (int)utcExpire.Subtract(epoch).TotalSeconds;

        //        Console.WriteLine($"The Token was issued at (UTC Time): {utcNow.ToString("yyyy/MM/dd")}");
        //        Console.WriteLine($"The Token will expire at (UTC Time): {utcExpire.ToString("yyyy/MM/dd")}\n");

        //        var headers = new Dictionary<string, object>
        //        {
        //            { "alg", algorithm },
        //            { "kid", keyId }
        //        };
        //        var payload = new Dictionary<string, object>
        //        {
        //            { "iss", teamId },
        //            { "iat", epochNow },
        //            { "exp", epochExpire }
        //        };

        //        var headersString = string.Join($",\n{" ",4}", headers.Select(kv => $"{kv.Key}: {kv.Value}"));
        //        Console.WriteLine($"Headers to be encoded:");
        //        Console.WriteLine($"{{\n{" ",4}{headersString}\n}}\n");

        //        var payloadString = string.Join($",\n{" ",4}", payload.Select(kv => $"{kv.Key}: {kv.Value}"));
        //        Console.WriteLine($"Payload to be encoded:");
        //        Console.WriteLine($"{{\n{" ",4}{payloadString}\n}}\n");

        //        var parameters = GetPrivateParameters();
        //        var secretKey = ECDsa.Create(parameters);
        //        //var token = JWT.Encode(payload, secretKey, JwsAlgorithm.ES256, headers);

        //        //var token = new JwtSecurityToken(
        //        //     issuer: "your_issuer",
        //        //     audience: "your_audience",
        //        //     claims: claims,
        //        //     expires: DateTime.Now.AddMinutes(30), // Token expiration time
        //        //     signingCredentials: creds
        //        // );



        //        var tokenHandler = new JwtSecurityTokenHandler();
        //        var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
        //        return tokenHandler.WriteToken(token);

        //        return token;
        //    }

        //static ECParameters GetPrivateParameters()
        //{
        //    using (var reader = File.OpenText("/path/to/your/MusicKit_Secret_Key.p8"))
        //    {
        //        var ecPrivateKeyParameters = (ECPrivateKeyParameters)new PemReader(reader).ReadObject();
        //        var x = ecPrivateKeyParameters.Parameters.G.AffineXCoord.GetEncoded();
        //        var y = ecPrivateKeyParameters.Parameters.G.AffineYCoord.GetEncoded();
        //        var d = ecPrivateKeyParameters.D.ToByteArrayUnsigned();

        //        var parameters = new ECParameters
        //        {
        //            Curve = ECCurve.NamedCurves.nistP256,
        //            D = d,
        //            Q = new ECPoint
        //            {
        //                X = x,
        //                Y = y
        //            }
        //        };

        //        return parameters;
        //    }
        //}

    }
}
