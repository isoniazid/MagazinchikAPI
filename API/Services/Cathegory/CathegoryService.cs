using FluentValidation;
using MagazinchikAPI.DTO;
using MagazinchikAPI.Infrastructure;
using MagazinchikAPI.Model;

namespace MagazinchikAPI.Services
{
    public class CathegoryService : ICathegoryService
    {

        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IValidator<CathegoryDtoCreate> _cathegoryDtoCreateValidator;
        private readonly int MAX_COUNT_RANDOM = 50; //Максимальное количество продуктов в странице
        public CathegoryService(ApplicationDbContext context, IMapper mapper, IValidator<CathegoryDtoCreate> CathegoryDtoCreateValidator)
        {
            _cathegoryDtoCreateValidator = CathegoryDtoCreateValidator;
            _context = context;
            _mapper = mapper;
        }


        public async Task<CathegoryDtoCreated> CreateCathegory(CathegoryDtoCreate input)
        {
            var validation = _cathegoryDtoCreateValidator.Validate(input);
            if (!validation.IsValid) throw new ValidatorException(validation);


            //NB ADD CHECK IF THIS IS ADMIN
            /*
            CHECKING IF THE REQUESTER IS ADMIN....
            */

            if (_context.Cathegories.FirstOrDefault(x => x.Name == input.Name && x.ParentId == input.ParentId) != null)
                throw new APIException("Cathegory already exists", 400);

            var cathegoryToSave = _mapper.Map<Model.Cathegory>(input);


            FindAllParents(cathegoryToSave);

            if (cathegoryToSave.Parent != null && !cathegoryToSave.Parent.IsParent)
                throw new APIException($"The Cathegory {cathegoryToSave.Parent.Name} cant't be a parent", 400);

            _context.Cathegories.Add(cathegoryToSave);
            await _context.SaveChangesAsync();



            return _mapper.Map<CathegoryDtoCreated>(cathegoryToSave);

        }


        public List<CathegoryDtoBaseInfo> GetRandomCathegories(int count)
        {
            if (count > MAX_COUNT_RANDOM) throw new APIException($"Too big amount for one query: {count}", 400);


            var query = $"SELECT * FROM \"Cathegories\" WHERE \"IsParent\" = FALSE ORDER BY RANDOM() LIMIT {count};";

            var result = _context.Cathegories.FromSqlRaw(query).ToList();
            for (int i = 0; i < result.Count; ++i)
            {
                FindAllParents(result[i]);
            }

            return _mapper.Map<List<CathegoryDtoBaseInfo>>(result);
        }

        private void FindAllParents(Cathegory cathegory)
        {
            cathegory.Parent = _context.Cathegories.Find(cathegory.ParentId);
            if (cathegory.Parent != null) FindAllParents(cathegory.Parent);
        }

    }
}