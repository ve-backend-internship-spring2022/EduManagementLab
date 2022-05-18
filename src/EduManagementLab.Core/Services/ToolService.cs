using EduManagementLab.Core.Entities;
using EduManagementLab.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduManagementLab.Core.Services
{
    public partial class ToolService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ToolService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<Tool> GetTools()
        {
            return _unitOfWork.Tools.GetAll();
        }
        public Tool GetTool(Guid id)
        {
            return _unitOfWork.Tools.GetById(id);
        }
        public Tool UpdateTool(Guid id, Tool tool)
        {
            var targetTool = _unitOfWork.Tools.GetById(id);
            targetTool.Name = tool.Name;
            targetTool.DeepLinkingLaunchUrl = tool.DeepLinkingLaunchUrl;
            targetTool.LoginUrl = tool.LoginUrl;
            targetTool.LaunchUrl = tool.LaunchUrl;
            targetTool.CustomProperties = tool.CustomProperties;
            targetTool.IdentityServerClientId = tool.IdentityServerClientId;
            targetTool.DeploymentId = tool.DeploymentId;
            _unitOfWork.Complete();

            return targetTool;
        }
        public Tool CreateTool(Tool tool)
        {
            var tools = _unitOfWork.Tools.GetAll();
            if (!tools.Any(c => c.IdentityServerClientId == tool.IdentityServerClientId))
            {
                _unitOfWork.Tools.Add(tool);
                _unitOfWork.Complete();
            }

            return tool;
        }
        public Tool DeleteTool(Guid ToolId)
        {
            var targetTool = _unitOfWork.Tools.GetById(ToolId);
            _unitOfWork.Tools.Remove(targetTool);
            _unitOfWork.Complete();
            return targetTool;
        }
    }
}
