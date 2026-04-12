using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class ConfiguracaoDeliveryService : IConfiguracaoDeliveryService
    {
        private readonly ApplicationDbContext _context;

        public ConfiguracaoDeliveryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ConfiguracaoDelivery>> ListarTodosAsync()
        {
            return await _context.ConfiguracoesDelivery.ToListAsync();
        }

        public async Task<ConfiguracaoDelivery?> BuscarPorIdAsync(Guid id)
        {
            return await _context.ConfiguracoesDelivery.FindAsync(id);
        }

        public async Task<(ConfiguracaoDelivery? config, string? erro)> CriarAsync(ConfiguracaoDelivery config)
        {
            if (config.Tipo == TipoDelivery.Proprio && await ExisteDeliveryProprioAsync())
                return (null, "Já existe uma configuração de delivery próprio. Edite a existente.");

            _context.ConfiguracoesDelivery.Add(config);
            await _context.SaveChangesAsync();
            return (config, null);
        }

        public async Task<ConfiguracaoDelivery?> EditarAsync(Guid id, ConfiguracaoDelivery config)
        {
            var existente = await _context.ConfiguracoesDelivery.FindAsync(id);
            if (existente == null) return null;

            existente.Tipo = config.Tipo;
            existente.NomeApp = config.NomeApp;
            existente.ComissaoPorcentagem = config.ComissaoPorcentagem;
            existente.TaxaAdicionalApp = config.TaxaAdicionalApp;
            existente.TaxaFixaProprio = config.TaxaFixaProprio;
            existente.Ativo = config.Ativo;

            await _context.SaveChangesAsync();
            return existente;
        }

        public async Task<bool> ExcluirAsync(Guid id)
        {
            var config = await _context.ConfiguracoesDelivery.FindAsync(id);
            if (config == null) return false;
            _context.ConfiguracoesDelivery.Remove(config);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExisteDeliveryProprioAsync()
        {
            return await _context.ConfiguracoesDelivery
                .AnyAsync(c => c.Tipo == TipoDelivery.Proprio);
        }
    }
}
