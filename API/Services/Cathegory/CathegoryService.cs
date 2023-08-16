using FluentValidation;
using MagazinchikAPI.DTO;
using MagazinchikAPI.DTO.Cathegory;
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

            if (_context.Cathegories.FirstOrDefault(x => x.Name == input.Name && x.ParentId == input.ParentId) is not null)
                throw new APIException("Cathegory already exists", 400);

            var cathegoryToSave = _mapper.Map<Model.Cathegory>(input);


            FindAllParents(cathegoryToSave);

            if (cathegoryToSave.Parent is not null && !cathegoryToSave.Parent.IsParent)
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

        public async Task<List<CathegoryDtoDescendants>> GetAll()
        {
            var rawResult = await _context.Cathegories.Where(x => x.ParentId != null).ToListAsync();

            foreach(var element in rawResult) await FindAllDescendants(element);

            return _mapper.Map<List<CathegoryDtoDescendants>>(rawResult);
        }

        public async Task<CathegoryDtoDescendants> GetByIdDescendants(long cathegoryId)
        {
            var cathegory = await _context.Cathegories.Include(x => x.Parent).FirstOrDefaultAsync(x => x.Id == cathegoryId)
            ?? throw new APIException("Invalid cathegory", 404);

            //if(!cathegory.IsParent) throw new APIException("This is the youngest category", 400);

            await FindAllDescendants(cathegory);

            return _mapper.Map<CathegoryDtoDescendants>(cathegory);
        }

        public async Task<CathegoryDtoBaseInfo> GetByIdParents(long cathegoryId)
        {
            var rawResult = await _context.Cathegories.FindAsync(cathegoryId)
            ?? throw new APIException("No such category", 404);

            FindAllParents(rawResult);

            return _mapper.Map<CathegoryDtoBaseInfo>(rawResult);
        }



        private async Task FindAllDescendants(Cathegory cathegory)
        {
            var descendants = await _context.Cathegories.Where(x => x.ParentId == cathegory.Id).ToListAsync();

            if (descendants.IsNullOrEmpty()) return;

            cathegory.Descendants = descendants;

            foreach (var descendant in descendants)
            {
                await FindAllDescendants(descendant);
            }

        }

        private void FindAllParents(Cathegory cathegory)
        {
            cathegory.Parent = _context.Cathegories.Find(cathegory.ParentId);
            if (cathegory.Parent is not null) FindAllParents(cathegory.Parent);
        }

    }
}