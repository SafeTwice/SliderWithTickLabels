using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SliderWithTickLabels
{
	/// <summary>
	/// Siga os passos 1a ou 1b e depois 2 para usar esse controle personalizado em um arquivo XAML.
	///
	/// Passo 1a) Usando o controle personalizado em um arquivo XAML que já existe no projeto atual.
	/// Adicione o atributo XmlNamespace ao elemento raiz do arquivo de marcação onde ele 
	/// deve ser usado:
	///
	///     xmlns:MyNamespace="clr-namespace:SliderWithTickLabels"
	///
	///
	/// Passo 1b) Usando o controle personalizado em um arquivo XAML que existe em um projeto diferente.
	/// Adicione o atributo XmlNamespace ao elemento raiz do arquivo de marcação onde ele 
	/// deve ser usado:
	///
	///     xmlns:MyNamespace="clr-namespace:SliderWithTickLabels;assembly=SliderWithTickLabels"
	///
	/// Também será necessário adicionar nesse projeto uma referência ao projeto que contém esse arquivo XAML
	/// e Recompilar para evitar erros de compilação:
	///
	///     No Gerenciador de Soluções, clique com o botão direito no projeto alvo e
	///     "Adicionar Referência"->"Projetos"->[Selecione esse projeto]
	///
	///
	/// Passo 2)
	/// Vá em frente e use seu controle no arquivo XAML.
	///
	///     <MyNamespace:CustomControl1/>
	///
	/// </summary>
	public class SliderWithTickLabels : Slider
	{
		private static readonly DependencyPropertyKey InternalTickLabelsPropertyKey;
		
		public static readonly DependencyProperty TickLabelsProperty;
		public static readonly DependencyProperty TickLabelTemplateProperty;
		public static readonly DependencyProperty TickLabelFrequencyProperty;

		public static readonly DependencyProperty ThumbWidthProperty;
		public static readonly DependencyProperty ThumbHeightProperty;

		[Bindable(false)]
		[Browsable(false)]
		public DoubleCollection InternalTickLabels
		{
			get
			{
				return (DoubleCollection) base.GetValue(SliderWithTickLabels.InternalTickLabelsPropertyKey.DependencyProperty);
			}
			private set
			{
				base.SetValue(SliderWithTickLabels.InternalTickLabelsPropertyKey, value);
			}
		}

		[Bindable(true)]
		[Category("Appearance")]
		public DoubleCollection TickLabels
		{
			get
			{
				return (DoubleCollection) base.GetValue(SliderWithTickLabels.TickLabelsProperty) ;
			}
			set
			{
				base.SetValue(SliderWithTickLabels.TickLabelsProperty, value);
			}
		}

		[Bindable(true)]
		public DataTemplate TickLabelTemplate
		{
			get
			{
				return (DataTemplate) base.GetValue(SliderWithTickLabels.TickLabelTemplateProperty) ;
			}
			set
			{
				base.SetValue(SliderWithTickLabels.TickLabelTemplateProperty, value);
			}
		}

		[Bindable(true)]
		[Category("Appearance")]
		public double TickLabelFrequency
		{
			get
			{
				return (double) base.GetValue(SliderWithTickLabels.TickLabelFrequencyProperty);
			}
			set
			{
				base.SetValue(SliderWithTickLabels.TickLabelFrequencyProperty, value);
			}
		}

		[Bindable(true)]
		[Category("Appearance")]
		public double ThumbWidth
		{
			get
			{
				return (double) base.GetValue(SliderWithTickLabels.ThumbWidthProperty);
			}
			set
			{
				base.SetValue(SliderWithTickLabels.ThumbWidthProperty, value);
			}
		}

		[Bindable(true)]
		[Category("Appearance")]
		public double ThumbHeight
		{
			get
			{
				return (double) base.GetValue(SliderWithTickLabels.ThumbHeightProperty);
			}
			set
			{
				base.SetValue(SliderWithTickLabels.ThumbHeightProperty, value);
			}
		}

		static SliderWithTickLabels()
		{
			SliderWithTickLabels.InternalTickLabelsPropertyKey = DependencyProperty.RegisterReadOnly("InternalTickLabels", typeof(DoubleCollection), typeof(SliderWithTickLabels),
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
			SliderWithTickLabels.TickLabelsProperty = DependencyProperty.Register("TickLabels", typeof(DoubleCollection), typeof(SliderWithTickLabels),
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
			SliderWithTickLabels.TickLabelTemplateProperty = DependencyProperty.Register("TickLabelTemplate", typeof(DataTemplate), typeof(SliderWithTickLabels),
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));
			SliderWithTickLabels.TickLabelFrequencyProperty = DependencyProperty.Register("TickLabelFrequency", typeof(double), typeof(SliderWithTickLabels),
				new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.AffectsRender ));

			SliderWithTickLabels.ThumbWidthProperty = DependencyProperty.Register( "ThumbWidth", typeof(double), typeof(SliderWithTickLabels),
				new FrameworkPropertyMetadata(11.0, FrameworkPropertyMetadataOptions.AffectsRender));
			SliderWithTickLabels.ThumbHeightProperty = DependencyProperty.Register( "ThumbHeight", typeof(double), typeof(SliderWithTickLabels),
				new FrameworkPropertyMetadata(18.0, FrameworkPropertyMetadataOptions.AffectsRender));

			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(SliderWithTickLabels), new FrameworkPropertyMetadata(typeof(SliderWithTickLabels)));
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			var propertyNames = new string[]
			{
				nameof(Minimum),
				nameof(Maximum),
				nameof(TickFrequency),
				nameof(TickLabelFrequency),
				nameof(Ticks),
				nameof(IsDirectionReversed)
			};

			if (IsInitialized && propertyNames.Contains(e.Property.Name))
			{
				CalculateTickLabels();
			}
		}

		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);

			CalculateTickLabels();
		}

		protected override Geometry GetLayoutClip(Size layoutSlotSize)
		{
			return ClipToBounds ? base.GetLayoutClip(layoutSlotSize) : null;
		}

		private void CalculateTickLabels()
		{
			if (this.TickLabels != null && this.TickLabels.Any())
			{
				this.InternalTickLabels = new DoubleCollection(this.TickLabels.Union(new double[] { this.Minimum, this.Maximum }).Where(t => this.Minimum <= t && t <= this.Maximum));
			}
			else
			{
				double tickLabelFrequency = this.TickLabelFrequency;
				bool isValidTickLabelFrequency = IsValidDoubleValue(tickLabelFrequency);

				if (!isValidTickLabelFrequency && this.Ticks != null && this.Ticks.Any())
				{
					this.InternalTickLabels = new DoubleCollection(this.Ticks.Union(new double[] { this.Minimum, this.Maximum }).Where(t => this.Minimum <= t && t <= this.Maximum ));
				}
				else
				{
					if (!isValidTickLabelFrequency)
					{
						tickLabelFrequency = this.TickFrequency;
					}

					if (tickLabelFrequency > 0.0)
					{
						this.InternalTickLabels = new DoubleCollection(
							Enumerable.Range(
								0,
								Convert.ToInt32(
									Math.Ceiling((this.Maximum - this.Minimum) / tickLabelFrequency)
								) + 1
							)
							.Select(t => Math.Min(t * tickLabelFrequency + this.Minimum, this.Maximum))
						);
					}
				}
			}
		}
		private static bool IsValidDoubleValue(object value)
		{
			double num = (double) value;
			return !double.IsNaN(num) && !double.IsInfinity(num);
		}
	}
}
