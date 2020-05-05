using System;
using System.Collections.Generic;
using System.Linq;

namespace PasswordCriteria
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Based on Google's criteria, inputPassword must contain:");
			Console.WriteLine("  - Must not have user name in inputPassword.");
			Console.WriteLine("  - Must be mixed case.");
			Console.WriteLine("  - Must have at least 1 numerical character.");
			Console.WriteLine("  - Must have at least 1 special (non-letter or non-numerical) character.");
			Console.WriteLine("  - Must not be a common inputPassword.");

			Console.Write("Please enter a inputPassword: ");

			var password = readPassword();

			while (!string.IsNullOrWhiteSpace(password))
			{
				checkPassword(password);

				Console.Write("Please enter a inputPassword: ");
				password = readPassword();
			}
		}

		private static string readPassword()
		{
			var password = string.Empty;

			do
			{
				var key = Console.ReadKey(true);

				if (key.Key == ConsoleKey.Enter)
				{
					Console.WriteLine();
					break;
				}
				else if (key.Key != ConsoleKey.Backspace)
				{
					Console.Write('*');
					password += key.KeyChar;
				}
				else
				{
					if (password.Length <= 0) continue;

					Console.Write("\b \b");
					password = password.Substring(0, password.Length - 1);
				}
			} while (true);

			Console.WriteLine();

			return password;
		}

		private static void checkPassword(string password)
		{
			var passwordChars = password.ToList();
			var passwordValid = true;

			// Character count must be greater than 6.
			if (passwordChars.Count < 6)
			{
				Console.WriteLine("Password invalid. Must contain at least 6 characters.");
				passwordValid = false;
			}

			// Must be mixed case.
			var hasLowerCaseChars = passwordChars.Any(char.IsLower);
			var hasUpperCaseChars = passwordChars.Any(char.IsUpper);

			if (!hasLowerCaseChars || !hasUpperCaseChars)
			{
				Console.WriteLine("Password invalid. Must be mixed case.");
				passwordValid = false;
			}

			// Must contain at least 1 numerical character.
			if (!passwordChars.Any(char.IsDigit))
			{
				Console.WriteLine("Password invalid. Must contain at least 1 numerical character.");
				passwordValid = false;
			}

			// Must contain at least 1 special (non-letter or non-numerical) character.
			var hasSpecialChars = passwordChars
				.Any(x => !char.IsLetterOrDigit(x));

			if (!hasSpecialChars)
			{
				Console.WriteLine("Password invalid. Must contain at least 1 special (non-letter or non-numerical) character.");
				passwordValid = false;
			}

			// Must not be a common inputPassword (tests against 500 known common passwords)
			if (password.Length > 6 && commonPasswords().Any(x => checkCommonVariations(x, password)))
			{
				Console.WriteLine("Password invalid. Must not be a common (or variation of a common) password.");
				passwordValid = false;
			}

			// If criteria succeeds.
			if (passwordValid)
			{
				Console.WriteLine($"Password ({password}) Valid.");
			}

			Console.WriteLine();
		}

		public static bool checkCommonVariations(string commonPassword, string inputPassword)
		{
			if (inputPassword == commonPassword)
				return true;

			var referencePassword = inputPassword;

			// Check for common character replacements (i.e. "leet"-speak)
			referencePassword = referencePassword.Replace('@', 'a');
			referencePassword = referencePassword.Replace('8', 'b');
			referencePassword = referencePassword.Replace('3', 'e');
			referencePassword = referencePassword.Replace('9', 'g');
			referencePassword = referencePassword.Replace('1', 'i');
			referencePassword = referencePassword.Replace('!', 'i');
			referencePassword = referencePassword.Replace('!', 'l');
			referencePassword = referencePassword.Replace('0', 'o');
			referencePassword = referencePassword.Replace('$', 's');
			referencePassword = referencePassword.Replace('7', 't');
			referencePassword = referencePassword.Replace('$', '5');
			// Database passwords are all in lower case.
			referencePassword = referencePassword.ToLower();

			// Ensures password doesn't have 
			var referencePasswordVariations = new List<string>
			{
				referencePassword,
				referencePassword.Substring(1, referencePassword.Length - 1), // Removes first character
				referencePassword.Substring(2, referencePassword.Length - 2), // Removes first 2 characters
				referencePassword.Substring(0, referencePassword.Length - 1), // Removes last character
				referencePassword.Substring(0, referencePassword.Length - 2) // removes last 2 characters
			};

			return referencePasswordVariations.Contains(commonPassword);
		}

		public static IEnumerable<string> commonPasswords()
		{
			// List of top 500 common passwords retrieved from:
			// https://github.com/danielmiessler/SecLists/blob/master/Passwords/Common-Credentials/500-worst-passwords.txt
			return new List<string>
			{
				"123456","password","12345678","qwerty","123456789","12345","1234","111111","1234567","dragon","123123","baseball","abc123","football","monkey","letmein","696969","shadow","master","666666","qwertyuiop","123321","mustang","1234567890","michael","654321","pussy","superman","1qaz2wsx","7777777","fuckyou","121212","000000","qazwsx","123qwe","killer","trustno1","jordan","jennifer","zxcvbnm","asdfgh","hunter","buster","soccer","harley","batman","andrew","tigger","sunshine","iloveyou","fuckme","2000","charlie","robert","thomas","hockey","ranger","daniel","starwars","klaster","112233","george","asshole","computer","michelle","jessica","pepper","1111","zxcvbn","555555","11111111","131313","freedom","777777","pass","fuck","maggie","159753","aaaaaa","ginger","princess","joshua","cheese","amanda","summer","love","ashley","6969","nicole","chelsea","biteme","matthew","access","yankees","987654321","dallas","austin","thunder","taylor","matrix","william","corvette","hello","martin","heather","secret","fucker","merlin","diamond","1234qwer","gfhjkm","hammer","silver","222222","88888888","anthony","justin","test","bailey","q1w2e3r4t5","patrick","internet","scooter","orange","11111","golfer","cookie","richard","samantha","bigdog","guitar","jackson","whatever","mickey","chicken","sparky","snoopy","maverick","phoenix","camaro","sexy","peanut","morgan","welcome","falcon","cowboy","ferrari","samsung","andrea","smokey","steelers","joseph","mercedes","dakota","arsenal","eagles","melissa","boomer","booboo","spider","nascar","monster","tigers","yellow","xxxxxx","123123123","gateway","marina","diablo","bulldog","qwer1234","compaq","purple","hardcore","banana","junior","hannah","123654","porsche","lakers","iceman","money","cowboys","987654","london","tennis","999999","ncc1701","coffee","scooby","0000","miller","boston","q1w2e3r4","fuckoff","brandon","yamaha","chester","mother","forever","johnny","edward","333333","oliver","redsox","player","nikita","knight","fender","barney","midnight","please","brandy","chicago","badboy","iwantu","slayer","rangers","charles","angel","flower","bigdaddy","rabbit","wizard","bigdick","jasper","enter","rachel","chris","steven","winner","adidas","victoria","natasha","1q2w3e4r","jasmine","winter","prince","panties","marine","ghbdtn","fishing","cocacola","casper","james","232323","raiders","888888","marlboro","gandalf","asdfasdf","crystal","87654321","12344321","sexsex","golden","blowme","bigtits","8675309","panther","lauren","angela","bitch","spanky","thx1138","angels","madison","winston","shannon","mike","toyota","blowjob","jordan23","canada","sophie","Password","apples","dick","tiger","razz","123abc","pokemon","qazxsw","55555","qwaszx","muffin","johnson","murphy","cooper","jonathan","liverpoo","david","danielle","159357","jackie","1990","123456a","789456","turtle","horny","abcd1234","scorpion","qazwsxedc","101010","butter","carlos","password1","dennis","slipknot","qwerty123","booger","asdf","1991","black","startrek","12341234","cameron","newyork","rainbow","nathan","john","1992","rocket","viking","redskins","butthead","asdfghjkl","1212","sierra","peaches","gemini","doctor","wilson","sandra","helpme","qwertyui","victor","florida","dolphin","pookie","captain","tucker","blue","liverpool","theman","bandit","dolphins","maddog","packers","jaguar","lovers","nicholas","united","tiffany","maxwell","zzzzzz","nirvana","jeremy","suckit","stupid","porn","monica","elephant","giants","jackass","hotdog","rosebud","success","debbie","mountain","444444","xxxxxxxx","warrior","1q2w3e4r5t","q1w2e3","123456q","albert","metallic","lucky","azerty","7777","shithead","alex","bond007","alexis","1111111","samson","5150","willie","scorpio","bonnie","gators","benjamin","voodoo","driver","dexter","2112","jason","calvin","freddy","212121","creative","12345a","sydney","rush2112","1989","asdfghjk","red123","bubba","4815162342","passw0rd","trouble","gunner","happy","fucking","gordon","legend","jessie","stella","qwert","eminem","arthur","apple","nissan","bullshit","bear","america","1qazxsw2","nothing","parker","4444","rebecca","qweqwe","garfield","01012011","beavis","69696969","jack","asdasd","december","2222","102030","252525","11223344","magic","apollo","skippy","315475","girls","kitten","golf","copper","braves","shelby","godzilla","beaver","fred","tomcat","august","buddy","airborne","1993","1988","lifehack","qqqqqq","brooklyn","animal","platinum","phantom","online","xavier","darkness","blink182","power","fish","green","789456123","voyager","police","travis","12qwaszx","heaven","snowball","lover","abcdef","00000","pakistan","007007","walter","playboy","blazer","cricket","sniper","hooters","donkey","willow","loveme","saturn","therock","redwings"
			};
		}
	}
}
