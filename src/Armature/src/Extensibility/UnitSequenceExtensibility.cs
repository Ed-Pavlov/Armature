using System;
using Armature.Core;


namespace Armature.Extensibility
{
  public abstract class UnitSequenceExtensibility : IUnitSequenceExtensibility
  {
    protected readonly IScannerTree ScannerTree;

    protected UnitSequenceExtensibility(IScannerTree scannerTree)
      => ScannerTree = scannerTree ?? throw new ArgumentNullException(nameof(scannerTree));

    IScannerTree IUnitSequenceExtensibility.ScannerTree => ScannerTree;
  }
}
