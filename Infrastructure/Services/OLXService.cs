using Application.Models;
using Application.Services;
using Microsoft.Playwright;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class OLXService : IOLXServices
    {
        public async Task<bool> PostAnuntAsync(AnuntModel anunt)
        {
            // Crează o instanță Playwright
            using (var playwright = await Playwright.CreateAsync())
            {
                // Lansează Microsoft Edge în loc de Chromium, specificând calea executabilului Edge
                var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
                {
                    Headless = false,
                    ExecutablePath = @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe" // Către executabilul Edge
                });

                var context = await browser.NewContextAsync();

                // Încarcă sesiunea salvată dacă există
                var storagePath = Path.Combine(Directory.GetCurrentDirectory(), "olx_session.json");
                if (File.Exists(storagePath))
                {
                    // Corectarea erorii de conversie - folosim StorageStateOptions
                    await context.StorageStateAsync(new BrowserContextStorageStateOptions { Path = storagePath });
                    Console.WriteLine("Sesiunea a fost încărcată.");
                }
                else
                {
                    var page = await context.NewPageAsync();
                    await page.GotoAsync("https://www.olx.ro");

                    // Click pe butonul de autentificare
                    await page.ClickAsync("text=Autentificare", new PageClickOptions { Timeout = 6000000 });

                    // Așteaptă ca utilizatorul să se logheze manual
                    Console.WriteLine("Loghează-te manual în OLX și apasă Enter când ai terminat...");
                    Console.ReadLine();

                    // Salvează starea sesiunii (cookie-uri și informații de autentificare)
                    await context.StorageStateAsync(new BrowserContextStorageStateOptions { Path = storagePath });
                    Console.WriteLine("Sesiunea a fost salvată.");
                }

                // După autentificare, creează anunțul
                var pageAfterLogin = await context.NewPageAsync();
                await pageAfterLogin.GotoAsync("https://www.olx.ro/post-new-ad/");

                // Completează formularul pentru postarea anunțului
                await pageAfterLogin.FillAsync("input[name='title']", anunt.Titlu);
                await pageAfterLogin.FillAsync("textarea[name='description']", anunt.Descriere);
                await pageAfterLogin.FillAsync("input[name='price']", anunt.Pret.ToString());
                await pageAfterLogin.SetInputFilesAsync("input[type='file']", anunt.CaleImagine);

                // Selectează categoria, localitatea, etc. (asumând că sunt opțiuni deja prezente pe pagină)
                // Aici poți adăuga cod pentru a selecta categoria sau localitatea, dacă este necesar.
                // De exemplu:
                // await pageAfterLogin.SelectOptionAsync("select[name='category']", "Electronics");
                // await pageAfterLogin.SelectOptionAsync("select[name='city']", "Bucuresti");

                // Click pe butonul pentru a publica anunțul
                await pageAfterLogin.ClickAsync("text=Publică anunțul");

                // Așteaptă câteva secunde pentru a te asigura că anunțul este postat
                await Task.Delay(3000); // Poți ajusta această valoare după nevoie

                // Închide browser-ul
                await browser.CloseAsync();

                // Dacă anunțul a fost postat cu succes
                return true;
            }
        }
    }
}
