using System;
namespace ProspectingProject
{
    public interface IDracoreService
    {
        // Phone numbers only
        Consumer001 ByIdTYPE001(long prospectingIdNumber);
        // Email addresses only
        Consumer002 ByIdTYPE002(long prospectingIdNumber);
    }
}
