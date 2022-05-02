using EduManagementLab.Core.Entities;
using EduManagementLab.Core.Exceptions;
using EduManagementLab.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduManagementLab.Core.Services
{
    public partial class ResourceLinkService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ResourceLinkService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<IMSLTIResourceLink> GetResourceLinks()
        {
            return _unitOfWork.ResourceLinks.GetResourceLinks(true);
        }
        public IMSLTIResourceLink GetResourceLink(Guid id)
        {
            var resourceLink = _unitOfWork.ResourceLinks.GetResourceLink(id, true);
            if (resourceLink == null)
            {
                throw new IMSLTIResourceLinkNotFoundException(id);
            }
            return resourceLink;
        }
        public IMSLTIResourceLink CreateResourceLink(IMSLTIResourceLink resourceLink)
        {
            if (!_unitOfWork.ResourceLinks.GetResourceLinks(true).Any(c => c.Tool.Id == resourceLink.Tool.Id || c.Title == resourceLink.Title))
            {
                _unitOfWork.ResourceLinks.Add(resourceLink);
                _unitOfWork.Complete();
                return resourceLink;
            }
            else
            {
                throw new IMSLTIResourceLinkToolDublicatedException(resourceLink.Tool.Name);
            }

        }
        public IMSLTIResourceLink DeleteResourceLink(Guid resourceId)
        {
            var targetResourceLink = _unitOfWork.ResourceLinks.GetById(resourceId);
            _unitOfWork.ResourceLinks.Remove(targetResourceLink);
            _unitOfWork.Complete();
            return targetResourceLink;
        }
        public IMSLTIResourceLink UpdateResourceLink(Guid id, IMSLTIResourceLink newResource)
        {
            var targetResourceLink = _unitOfWork.ResourceLinks.GetById(id);
            targetResourceLink.Title = newResource.Title;
            targetResourceLink.Description = newResource.Description;
            targetResourceLink.CustomProperties = newResource.CustomProperties;
            targetResourceLink.Tool = newResource.Tool;

            _unitOfWork.ResourceLinks.Update(targetResourceLink);
            _unitOfWork.Complete();
            return targetResourceLink;
        }
    }
}
