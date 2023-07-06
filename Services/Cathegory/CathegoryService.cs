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

            if(_context.Cathegories.FirstOrDefault(x => x.Name == input.Name) != null)
            throw new APIException("Cathegory already exists", 400);

            var cathegoryToSave =_mapper.Map<Model.Cathegory>(input);

            
            FindAllParents(cathegoryToSave);

            _context.Cathegories.Add(cathegoryToSave);
            await _context.SaveChangesAsync();
            
            

            return _mapper.Map<CathegoryDtoCreated>(cathegoryToSave);
        
        }

        private void FindAllParents(Cathegory cathegory)
        {
            cathegory.Parent = _context.Cathegories.Find(cathegory.ParentId);
            if(cathegory.Parent != null) FindAllParents(cathegory.Parent);
        }
        
    }
}