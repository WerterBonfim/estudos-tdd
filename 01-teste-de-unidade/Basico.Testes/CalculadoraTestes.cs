using System;
using Xunit;

namespace Basico.Testes
{
    public class CalculadoraTestes
    {
        [Fact]
        public void Somar()
        {
            // Arrange
            var calculadora = new Calculadora();
            
            // Act
            var resultado = calculadora.Somar(2, 2);
            
            // Assert
            Assert.Equal(4, resultado);
        }

        [Theory]
        [InlineData(1, 1, 2)]
        [InlineData(2, 2, 4)]
        [InlineData(4, 4, 8)]
        [InlineData(1, 3, 4)]
        public void SomarValores(double valor1, double valor2, double resultadoEsperado)
        {
            // Arrange
            var calculadora = new Calculadora();
            
            // Act
            var resultado = calculadora.Somar(valor1, valor2);
            
            // Assert
            Assert.Equal(resultadoEsperado, resultado);
        }
    }
}