using FluentValidation;
using MagazinchikAPI.DTO;
using MagazinchikAPI.DTO.Category;
using MagazinchikAPI.Infrastructure;
using MagazinchikAPI.Model;

namespace MagazinchikAPI.Services
{
    public class CategoryService : ICategoryService
    {

        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IValidator<CategoryDtoCreate> _categoryDtoCreateValidator;
        private readonly int MAX_COUNT_RANDOM = 50; //Максимальное количество продуктов в странице
        public CategoryService(ApplicationDbContext context, IMapper mapper, IValidator<CategoryDtoCreate> CategoryDtoCreateValidator)
        {
            _categoryDtoCreateValidator = CategoryDtoCreateValidator;
            _context = context;
            _mapper = mapper;
        }


        public async Task<CategoryDtoCreated> CreateCategory(CategoryDtoCreate input)
        {
            var validation = _categoryDtoCreateValidator.Validate(input);
            if (!validation.IsValid) throw new ValidatorException(validation);


            //NB ADD CHECK IF THIS IS ADMIN
            /*
            CHECKING IF THE REQUESTER IS ADMIN....
            */

            if (_context.Categories.FirstOrDefault(x => x.Name == input.Name && x.ParentId == input.ParentId) is not null)
                throw new APIException("Category already exists", 400);

            var categoryToSave = _mapper.Map<Model.Category>(input);


            FindAllParents(categoryToSave);

            if (categoryToSave.Parent is not null && !categoryToSave.Parent.IsParent)
                throw new APIException($"The Category {categoryToSave.Parent.Name} cant't be a parent", 400);

            _context.Categories.Add(categoryToSave);
            await _context.SaveChangesAsync();



            return _mapper.Map<CategoryDtoCreated>(categoryToSave);

        }


        public List<CategoryDtoBaseInfo> GetRandomCategories(int count)
        {
            if (count > MAX_COUNT_RANDOM) throw new APIException($"Too big amount for one query: {count}", 400);


            var query = $"SELECT * FROM \"Categories\" WHERE \"IsParent\" = FALSE ORDER BY RANDOM() LIMIT {count};";

            var result = _context.Categories.FromSqlRaw(query).ToList();
            for (int i = 0; i < result.Count; ++i)
            {
                FindAllParents(result[i]);
            }

            return _mapper.Map<List<CategoryDtoBaseInfo>>(result);
        }

        public async Task<List<CategoryDtoDescendants>> GetAll()
        {
            var rawResult = await _context.Categories.Where(x => x.ParentId != null).ToListAsync();

            foreach(var element in rawResult) await FindAllDescendants(element);

            return _mapper.Map<List<CategoryDtoDescendants>>(rawResult);
        }

        public async Task<CategoryDtoDescendants> GetByIdDescendants(long categoryId)
        {
            var category = await _context.Categories.Include(x => x.Parent).FirstOrDefaultAsync(x => x.Id == categoryId)
            ?? throw new APIException("Invalid category", 404);

            //if(!category.IsParent) throw new APIException("This is the youngest category", 400);

            await FindAllDescendants(category);

            return _mapper.Map<CategoryDtoDescendants>(category);
        }

        public async Task<CategoryDtoBaseInfo> GetByIdParents(long categoryId)
        {
            var rawResult = await _context.Categories.FindAsync(categoryId)
            ?? throw new APIException("No such category", 404);

            FindAllParents(rawResult);

            return _mapper.Map<CategoryDtoBaseInfo>(rawResult);
        }



        private async Task FindAllDescendants(Category category)
        {
            var descendants = await _context.Categories.Where(x => x.ParentId == category.Id).ToListAsync();

            if (descendants.IsNullOrEmpty()) return;

            category.Descendants = descendants;

            foreach (var descendant in descendants)
            {
                await FindAllDescendants(descendant);
            }

        }

        private void FindAllParents(Category category)
        {
            category.Parent = _context.Categories.Find(category.ParentId);
            if (category.Parent is not null) FindAllParents(category.Parent);
        }

    }
}