using AutoMapper;
using Kopilych.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Persistence.Services
{
	public class AutoMapperService : IMapperService
	{
		private readonly IMapper _mapper;

		public AutoMapperService(IMapper mapper)
		{
			_mapper = mapper;
		}

		public TDestination Map<TSource, TDestination>(TSource source)
		{
			return _mapper.Map<TSource, TDestination>(source);
		}
	}

}
